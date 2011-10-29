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
        private readonly CameraDriver i;

        public ImagerBase SelectedCamera
        {
            get;
            private set;
        }

        public double Sensitivity
        {
            get { return SensitivitySlider.Value; }
            set { SensitivitySlider.Value = value; }
        }

        // basically forward assignments to the actual overlay
        public Rect SelectedBoundingRect
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

        public MotionDetectorConfig(ImagerBase selectedCamera)
        {
            InitializeComponent();
            i = CameraDriver.Instance;
            if (selectedCamera != null)
            {
                SelectedCamera = selectedCamera;
                DeviceList.SelectedIndex = selectedCamera.Info.DeviceId;
            }
            Init();
        }

        public void Init()
        {
            // populate device list
            Logger.WriteLine("OnLoad");

            foreach (Camera c in i.CamerasAvailable)
            {
                DeviceList.Items.Add(c);
            }

            if (SelectedCamera == null)
            {
                DeviceList.SelectedIndex = 0;
            }

            if (i.DeviceCount > 0)
            {
                int cameraIndex = (SelectedCamera != null && SelectedCamera.Info.DeviceId < i.DeviceCount) ? SelectedCamera.Info.DeviceId : 0;

                // start the camera 0 if it isn't already running
                SelectedCamera = i.CamerasAvailable[cameraIndex];

                DeviceList.SelectedIndex = cameraIndex;
                Logger.WriteLine("Selected Index " + DeviceList.SelectedIndex);

                // TODO: Move the check into Camera
                
                if (!SelectedCamera.Running)
                {                
                    ThreadPool.QueueUserWorkItem(o => SelectedCamera.StartFrameGrabbing());
                }

                SelectedCamera.OnImageUpdated -= i_OnImageUpdated;
                SelectedCamera.OnImageUpdated += i_OnImageUpdated;

                Logger.WriteLine("using " + SelectedCamera.Info);

                // allow saving in this state
                CanSave = true;
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
            Dispatcher.Invoke(new Action(SetCameraImageSource), null);
        }

        /**<summary>
         * Does the actual drawing of the camera image to the WPF image object in the config window. 
         * </summary>
         */
        public virtual void SetCameraImageSource()
        {
            Bitmap backBuffer = SelectedCamera.ImageAsBitmap();

            // Convert the bitmap to BitmapSource for use with WPF controls
            IntPtr hBmp = backBuffer.GetHbitmap();

            camera_image.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBmp, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

            backBuffer.Dispose();
            VisionModulesWpfCommon.DeleteGDIObject(hBmp);
        }

        public override void OnClosing()
        {
            Logger.WriteLine("OnClosing");
            if (SelectedCamera != null)
            {
                SelectedCamera.OnImageUpdated -= i_OnImageUpdated;
                SelectedCamera.TryStopFrameGrabbing();
            }      
        }

        public override void OnSave()
        {
            SelectedCamera = DeviceList.SelectedItem as Camera;
        }

        private void DeviceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Camera selectedCam = i.CamerasAvailable[DeviceList.SelectedIndex];

            if (selectedCam != SelectedCamera)
            {
                Logger.WriteLine("Switching Cam to " + selectedCam.Info.Description);

                SelectedCamera.OnImageUpdated -= i_OnImageUpdated;
                if (SelectedCamera.Running)
                    SelectedCamera.TryStopFrameGrabbing();

                // switch the camera display
                SelectedCamera = selectedCam;
                SelectedCamera.OnImageUpdated -= i_OnImageUpdated;
                SelectedCamera.OnImageUpdated += i_OnImageUpdated;
                if (!SelectedCamera.Running)
                {
                    ThreadPool.QueueUserWorkItem(o => SelectedCamera.StartFrameGrabbing());
                }
            }
        }

        public override string Title
        {
            get
            {
                return "Motion Detector";
            }
        }
    }
}
