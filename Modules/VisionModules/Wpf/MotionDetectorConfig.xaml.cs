using System;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using MayhemCore;
using MayhemOpenCVWrapper;
using MayhemWpf.UserControls;

namespace VisionModules.Wpf
{
    /// <summary>
    /// Interaction logic for MotionDetectorConfig.xaml
    /// </summary>
    public partial class MotionDetectorConfig : WpfConfiguration
    {
        private CameraDriver i = CameraDriver.Instance;
      
        public Camera CameraSelected
        {
            get;
            private set; 
        }

        private delegate void VoidHandler();

        // basically forward assignments to the actual overlay
        public Rect selectedBoundingRect
        {
            get
            {
                return overlay.GetBoundingRect();
            }
            set
            {
                overlay.DisplayBoundingRect(value);
            }
        }

        public MotionDetectorConfig(Camera c)
        {
            CameraSelected = c;
            InitializeComponent();
            DeviceList.SelectedIndex = c.Info.DeviceId;
        }

        public override void OnLoad()
        {
            // populate device list
            Logger.WriteLine("OnLoad");

            foreach (Camera c in i.CamerasAvailable)
            {
                DeviceList.Items.Add(c);
            }

            if (CameraSelected == null)
            {
                DeviceList.SelectedIndex = 0;
            }

            if (i.DeviceCount > 0)
            {
                int camera_index = (CameraSelected != null && CameraSelected.Info.DeviceId < i.DeviceCount) ? CameraSelected.Info.DeviceId : 0;

                // start the camera 0 if it isn't already running
                CameraSelected = i.CamerasAvailable[camera_index];
               
                DeviceList.SelectedIndex = camera_index;
                Logger.WriteLine("Selected Index " + DeviceList.SelectedIndex);

                ///TODO: SVEN: Is it ok to comment out cam.running?
                /// NO, not ok at the moment!
                /// TODO: Move the check into Camera

                CameraSelected.OnImageUpdated -= i_OnImageUpdated;
                CameraSelected.OnImageUpdated += i_OnImageUpdated;
                if (!CameraSelected.Running)
                {             
                ThreadPool.QueueUserWorkItem(new WaitCallback((o) =>
                    {
                        CameraSelected.StartFrameGrabbing();
                    }));
                }
                Logger.WriteLine("using " + CameraSelected.Info.ToString());
                // alow saving in this state
                this.CanSave = true;
            }
            else
            {
                Logger.WriteLine("No camera available");
            }
        }

        /**<summary>
         * Invokes image source setting when a new image is available from the update handler. 
         * </summary>
         */
        public void i_OnImageUpdated(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new VoidHandler(SetCameraImageSource), null);
        }

        /**<summary>
         * Does the actual drawing of the camera image to the WPF image object in the config window. 
         * </summary>
         */
        public virtual void SetCameraImageSource()
        {
            Bitmap BackBuffer = CameraSelected.ImageAsBitmap();

            IntPtr hBmp;

            //Convert the bitmap to BitmapSource for use with WPF controls
            hBmp = BackBuffer.GetHbitmap();

            this.camera_image.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBmp, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            // this.camera_image.Source.Freeze();

            BackBuffer.Dispose();
            VisionModulesWPFCommon.DeleteGDIObject(hBmp);
        }

        public override void OnClosing()
        {
            Logger.WriteLine("OnClosing");
            foreach (Camera c in i.CamerasAvailable)
            {
                CameraSelected.OnImageUpdated -= i_OnImageUpdated;
                CameraSelected.TryStopFrameGrabbing();
                Thread.Sleep(200);
            }

            base.OnClosing();
        }

        public override void OnSave()
        {
            CameraSelected = DeviceList.SelectedItem as Camera;
        }

        private void DeviceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Logger.WriteLine("");
            Camera selected_cam = i.CamerasAvailable[DeviceList.SelectedIndex];

            if (selected_cam != CameraSelected)
            {
                Logger.WriteLine("Switching Cam to " + selected_cam.Info.ToString());

                CameraSelected.OnImageUpdated -= i_OnImageUpdated;
                if (CameraSelected.Running)
                    CameraSelected.TryStopFrameGrabbing();

                // switch the camera display
                CameraSelected = selected_cam;
                CameraSelected.OnImageUpdated -= i_OnImageUpdated;
                CameraSelected.OnImageUpdated += i_OnImageUpdated;
                if (!CameraSelected.Running)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback((o) =>
                    {
                        CameraSelected.StartFrameGrabbing();
                    }));
                }
            }
        }
    }
}
