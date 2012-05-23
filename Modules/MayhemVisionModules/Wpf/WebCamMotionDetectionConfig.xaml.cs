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
using MayhemWpf.UserControls;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using MayhemWebCamWrapper;

namespace MayhemVisionModules.Wpf
{
    /// <summary>
    /// Interaction logic for WebCamMotionDetectionConfig.xaml
    /// </summary>
    public partial class WebCamMotionDetectionConfig : WpfConfiguration
    {
    
        public WebCamMotionDetectionConfig()
        {
            InitializeComponent();
            SelectedCameraIndex = -1;
            _selectedState = false;
            camera_selector.CamerasReady += OnSelectorCamerasReady;
            camera_selector.CamerasNotReady += OnSelectorCamerasNotReady;
            camera_selector.InitSelector();
        }

        public WebCamMotionDetectionConfig(int cameraFocus, int cameraZoom, float percentageThresh, float timeThresh, int differenceThresh, int roix, int roiy, int roiwidth, int roiheight)
        {
            InitializeComponent();

            PercentageThresh = percentageThresh;
            DiffThresh = differenceThresh;
            TimeThresh = timeThresh;
            RoiX = roix;
            RoiY = roiy;
            RoiWidth = roiwidth;
            RoiHeight = roiheight;
            CameraZoom = cameraZoom;
            CameraFocus = cameraFocus;
            camera_roi_selector.SetSliderValues(cameraFocus, cameraZoom, percentageThresh, timeThresh, differenceThresh);
            camera_roi_selector.SetROI(roix, roiy, roiwidth, roiheight);
            SelectedCameraIndex = -1;
            _selectedState = false;
            camera_selector.CamerasReady += OnSelectorCamerasReady;
            camera_selector.CamerasNotReady += OnSelectorCamerasNotReady;
            camera_selector.InitSelector();
        }

        ~WebCamMotionDetectionConfig()
        {
            camera_selector.CamerasReady -= OnSelectorCamerasReady;
            camera_selector.CamerasNotReady -= OnSelectorCamerasNotReady;
        }

        private void OnSelectorCamerasReady(object o, EventArgs a)
        {
            ButtonState.IsEnabled = true;
            CanSave = true;

        }

        private void OnSelectorCamerasNotReady(object o, EventArgs a)
        {
            ButtonState.IsEnabled = false;
            CanSave = false;
        }

        public override void OnClosing()
        {
            camera_roi_selector.CleanUp();
            camera_selector.Cleanup();
        }

        public override void OnLoad()
        {
            camera_roi_selector.SetSliderValues(CameraFocus, CameraZoom, PercentageThresh, TimeThresh, DiffThresh); 
        }

        public override void OnSave()
        {
            PercentageThresh = (float)camera_roi_selector.CurrentPercentage;
            DiffThresh = (int)camera_roi_selector.CurrentDifference;
            TimeThresh = (float)camera_roi_selector.CurrentTime;
            RoiX = camera_roi_selector.RoiX;
            RoiY = camera_roi_selector.RoiY;
            RoiWidth = camera_roi_selector.RoiWidth;
            RoiHeight = camera_roi_selector.RoiHeight;
            CameraFocus = camera_roi_selector.CurrentFocus;
            CameraZoom = camera_roi_selector.CurrentZoom;
            SelectedCameraPath = camera_selector.GetSelectedCameraPath();
            SelectedCameraName = camera_selector.GetSelectedCameraName();
            SelectedCameraIndex = camera_selector.GetSelectedCameraIndex();
        }

       
        private void Button_Clicked(object sender, RoutedEventArgs e)
        {
            if (!_selectedState)
            {
                SelectedCameraIndex = camera_selector.GetSelectedCameraIndex();
                SelectedCameraName = WebcamManager.GetCamera(SelectedCameraIndex).WebCamName;
                SelectedCameraPath = WebcamManager.GetCamera(SelectedCameraIndex).WebCamPath;
                if (SelectedCameraIndex > -1 && SelectedCameraIndex < WebcamManager.NumberConnectedCameras())
                {
                    camera_roi_selector.SetImageSource(SelectedCameraIndex);
                }
                SelectorPanel.Visibility = Visibility.Collapsed;
                ROIPanel.Visibility = Visibility.Visible;
                ButtonState.Content = "<< Select another camera";
            }
            else
            {
                ROIPanel.Visibility = Visibility.Collapsed;
                camera_roi_selector.CleanUp();
                SelectorPanel.Visibility = Visibility.Visible;
                ButtonState.Content = "Configure motion detector >>";
            }
            _selectedState = !_selectedState;
        }

        public override string Title
        {
            get
            {
                return "Motion Detection";
            }
        }


        public int SelectedCameraIndex;
        public string SelectedCameraName;
        public string SelectedCameraPath;
        public int RoiX, RoiY, RoiWidth, RoiHeight;
        public float PercentageThresh, TimeThresh;
        public int DiffThresh;
        public int CameraFocus, CameraZoom;
        private bool _selectedState;
    }
}
