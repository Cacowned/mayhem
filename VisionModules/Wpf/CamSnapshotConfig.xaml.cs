/* CamSnapshotConfig.xaml.cs
 * 
 * Snapshot configuration window using OpenCV Camera Library
 * 
 * Authors: Sven Kratz, Eli White
 * (c) 2011 Mayhem Open Source Project by Microsoft inc. 
 * 
 * 
 */


using System.Windows;
using System.Windows.Forms;
using MayhemOpenCVWrapper;
using System.Diagnostics;
using System.Drawing;
using System;


namespace VisionModules.Wpf
{
	/// <summary>
	/// Interaction logic for WebcamSnapshotConfig.xaml
	/// </summary>
	public partial class CamSnapshotConfig : Window
	{
		public string location;
		// public Device captureDevice;

        private MayhemImageUpdater i = MayhemImageUpdater.Instance;
        private MayhemImageUpdater.ImageUpdateHandler imageUpdateHandler;


        private delegate void SetCameraImageSource();

		public CamSnapshotConfig(string location, object captureDevice) {
			this.location = location;
			

			InitializeComponent();

            // TODO: Enumerate devices
            imageUpdateHandler = new MayhemImageUpdater.ImageUpdateHandler(i_OnImageUpdated);

            // if the image update isn't running yet, start it (could be dangerous) 
            if (i.running == false)
            {
                i.StartFrameGrabbing();
            }

            // populate device list

            foreach(CameraInfo c in i.devices_available)
            {
                DeviceList.Items.Add(c);
            }

            DeviceList.SelectedIndex = 0; 
	

		}

        /**<summary>
        * Register / De-Register the image update handler if the window is visible / invisible
        * </summary>
        */
        private void Window_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.IsVisible)
            {
                i.OnImageUpdated += imageUpdateHandler;
            }
            else
            {
                i.OnImageUpdated -= imageUpdateHandler;
            }
        }


        /**<summary>
         * Invokes image source setting when a new image is available from the update handler. 
         * </summary>
         */
        public void i_OnImageUpdated(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new SetCameraImageSource(SetCameraImageSource_), null);
        }

        /**<summary>
         * Does the actual drawing of the camera image to the WPF image object in the config window. 
         * </summary>
         */
        private void SetCameraImageSource_()
        {
            Debug.WriteLine("[SetCameraImageSource_] ");

            //int stride = 320 * 3;
            Bitmap BackBuffer = new Bitmap(320, 240, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, 320, 240);

            // get at the bitmap data in a nicer way
            System.Drawing.Imaging.BitmapData bmpData =
                BackBuffer.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                BackBuffer.PixelFormat);

            int bufSize = i.bufSize;

            IntPtr ImgPtr = bmpData.Scan0;

            // grab the image


            lock (i.thread_locker)
            {
                // Copy the RGB values back to the bitmap
                System.Runtime.InteropServices.Marshal.Copy(i.imageBuffer, 0, ImgPtr, bufSize);
            }
            // Unlock the bits.
            BackBuffer.UnlockBits(bmpData);

            IntPtr hBmp;

            //Convert the bitmap to BitmapSource for use with WPF controls
            hBmp = BackBuffer.GetHbitmap();

            this.camera_preview.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBmp, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            this.camera_preview.Source.Freeze();
        }


		private void Button_Click_1(object sender, RoutedEventArgs e) {
			FolderBrowserDialog dlg = new FolderBrowserDialog();
			dlg.SelectedPath = location;

			if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK) {
				location = dlg.SelectedPath;
			}
		}

		private void Button_Save_Click(object sender, RoutedEventArgs e) {
			// captureDevice = DeviceList.SelectedItem as Device;
			DialogResult = true;
		}

		private void Button_Cancel_Click(object sender, RoutedEventArgs e) {
			DialogResult = false;
		}
	}
}
