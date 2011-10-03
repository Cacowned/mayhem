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
        protected Camera cam = null;
        public Camera selected_camera
        {
            get { return cam; }
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
            cam = c;
            InitializeComponent();
            DeviceList.SelectedIndex = c.Info.deviceId;
        }

        public override void OnLoad()
        {
            // populate device list
            Logger.WriteLine("OnLoad");

            foreach (Camera c in i.CamerasAvailable)
            {
                DeviceList.Items.Add(c);
            }

            if (cam == null)
            {
                DeviceList.SelectedIndex = 0;
            }

            if (i.DeviceCount > 0)
            {
                int camera_index = (selected_camera != null && selected_camera.Info.deviceId < i.DeviceCount) ? selected_camera.Info.deviceId : 0;

                // start the camera 0 if it isn't already running
                cam = i.CamerasAvailable[camera_index];
               
                DeviceList.SelectedIndex = camera_index;
                Logger.WriteLine("Selected Index " + DeviceList.SelectedIndex);

                ///TODO: SVEN: Is it ok to comment out cam.running?
                /// NO, not ok at the moment!
                /// TODO: Move the check into Camera

                cam.OnImageUpdated -= i_OnImageUpdated;
                cam.OnImageUpdated += i_OnImageUpdated;
                if (!cam.running)
                {             
                ThreadPool.QueueUserWorkItem(new WaitCallback((o) =>
                    {                   
                        cam.StartFrameGrabbing();
                    }));
                }
                Logger.WriteLine("using " + cam.Info.ToString());
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
            Bitmap BackBuffer = cam.ImageAsBitmap();

            IntPtr hBmp;

            //Convert the bitmap to BitmapSource for use with WPF controls
            hBmp = BackBuffer.GetHbitmap();

            this.camera_image.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBmp, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            // this.camera_image.Source.Freeze();

            BackBuffer.Dispose();
            VisionModulesWPFCommon.DeleteObject(hBmp);
        }

        public override void OnClosing()
        {
            Logger.WriteLine("OnClosing");
            foreach (Camera c in i.CamerasAvailable)
            {
                cam.OnImageUpdated -= i_OnImageUpdated;
                cam.TryStopFrameGrabbing();
                Thread.Sleep(200);
            }

            base.OnClosing();
        }

        public override void OnSave()
        {
            cam = DeviceList.SelectedItem as Camera;
        }

        private void DeviceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Logger.WriteLine("");
            Camera selected_cam = i.CamerasAvailable[DeviceList.SelectedIndex];

            if (selected_cam != cam)
            {
                Logger.WriteLine("Switching Cam to " + selected_cam.Info.FriendlyName());

                cam.OnImageUpdated -= i_OnImageUpdated;
                if (cam.running)
                    cam.TryStopFrameGrabbing();

                // switch the camera display
                cam = selected_cam;
                cam.OnImageUpdated -= i_OnImageUpdated;
                cam.OnImageUpdated += i_OnImageUpdated;
                if (!cam.running)
                {
                    ThreadPool.QueueUserWorkItem(new WaitCallback((o) =>
                    {
                        cam.StartFrameGrabbing();
                    }));
                }
            }
        }
    }
}
