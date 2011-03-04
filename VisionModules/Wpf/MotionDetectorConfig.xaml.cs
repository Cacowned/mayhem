using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MayhemOpenCVWrapper;
using System.Diagnostics;
using System.Drawing;

namespace VisionModules.Wpf
{
    /// <summary>
    /// Interaction logic for MotionDetectorConfig.xaml
    /// </summary>
    public partial class MotionDetectorConfig : Window
    {

        private MayhemCameraDriver i = MayhemCameraDriver.Instance;
        private Camera.ImageUpdateHandler imageUpdateHandler;
        private Camera cam; 

        private delegate void SetCameraImageSource();

        public MotionDetectorConfig()
        {
            InitializeComponent();

            // image1 = new System.Windows.Controls.Image();

            imageUpdateHandler = new Camera.ImageUpdateHandler(i_OnImageUpdated);

            // if the image update isn't running yet, start it (could be dangerous) 
            /*
            if (i.running == false)
            {
                i.StartFrameGrabbing();
            }*/

            // start the camera 0 if it isn't already running
            Camera cam = i.cameras_available[0];
            if (!cam.running) cam.StartFrameGrabbing();

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


        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
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

            this.camera_image.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBmp, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            this.camera_image.Source.Freeze();
        }

        private void overlay_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // TODO
        }

        private void overlay_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // TODO
        }

        private void overlay_MouseMove(object sender, MouseEventArgs e)
        {
            // TODO
        }

        private void overlay_MouseLeave(object sender, MouseEventArgs e)
        {
            // TODO
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

       


    }
}
