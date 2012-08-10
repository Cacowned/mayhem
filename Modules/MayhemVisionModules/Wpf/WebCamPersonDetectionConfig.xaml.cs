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
    /// Interaction logic for WebCamPersonDetectionConfig.xaml
    /// </summary>
    public partial class WebCamPersonDetectionConfig : WpfConfiguration
    {
        public WebCamPersonDetectionConfig()
        {
            InitializeComponent();
            SelectedCameraIndex = -1;
            _selectedState = false;
            FocusMinimum = 0;
            FocusMaximum = 1;
            FocusStep = 1;
            ZoomMinimum = 0;
            ZoomMaximum = 1;
            ZoomStep = 1;
            CurrentZoom = 1;
            CurrentFocus = 0;
            RoiX = 0;
            RoiY = 0;
            RoiWidth = 1;
            RoiHeight = 1;
            _selectedCameraIndex = -1;
            SelectedCameraIndex = -1;
            camera_selector.CamerasReady += OnSelectorCamerasReady;
            camera_selector.CamerasNotReady += OnSelectorCamerasNotReady;
            camera_selector.InitSelector();

        }

        public WebCamPersonDetectionConfig(int cameraFocus, int cameraZoom, double roix, double roiy, double roiwidth, double roiheight)
        {
            InitializeComponent();

            RoiX = roix;
            RoiY = roiy;
            RoiWidth = roiwidth;
            RoiHeight = roiheight;
            CameraZoom = cameraZoom;
            CameraFocus = cameraFocus;
            FocusMinimum = 0;
            FocusMaximum = 1;
            FocusStep = 1;
            ZoomMinimum = 0;
            ZoomMaximum = 1;
            ZoomStep = 1;
            CurrentZoom = 1;
            CurrentFocus = 0;
            RoiX = 0;
            RoiY = 0;
            RoiWidth = 1;
            RoiHeight = 1;
            _selectedCameraIndex = -1;
            SetSliderValues(cameraFocus, cameraZoom);
            SetROI(roix, roiy, roiwidth, roiheight);
            SelectedCameraIndex = -1;
            _selectedState = false;
            camera_selector.CamerasReady += OnSelectorCamerasReady;
            camera_selector.CamerasNotReady += OnSelectorCamerasNotReady;
            camera_selector.InitSelector();
        }

        ~WebCamPersonDetectionConfig()
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
            CleanUp();
            camera_selector.Cleanup();
        }

        public void Cleanup()
        {
            if (_selectedCameraIndex > -1)
            {
                WebCam camera = WebcamManager.GetCamera(_selectedCameraIndex);
                Image_Viewer.RemoveImageSource(camera);
                Image_Viewer.Clear();
                Image_Viewer_Orig.RemoveImageSource(camera);
            }
            camera_selector.Cleanup();
        }

        public override void OnLoad()
        {
            SetSliderValues(CameraFocus, CameraZoom); 
        }

        public override void OnSave()
        {
            SelectedCameraPath = camera_selector.GetSelectedCameraPath();
            SelectedCameraName = camera_selector.GetSelectedCameraName();
            SelectedCameraIndex = camera_selector.GetSelectedCameraIndex();
        }

       
        private void Button_Clicked(object sender, RoutedEventArgs e)
        {
            if (!_selectedState)
            {
                SelectedCameraIndex = _selectedCameraIndex = camera_selector.GetSelectedCameraIndex();
                SelectedCameraName = WebcamManager.GetCamera(SelectedCameraIndex).WebCamName;
                SelectedCameraPath = WebcamManager.GetCamera(SelectedCameraIndex).WebCamPath;
                if (SelectedCameraIndex > -1 && SelectedCameraIndex < WebcamManager.NumberConnectedCameras())
                {
                    Image_Viewer.SetImageSource(WebcamManager.GetCamera(_selectedCameraIndex));
                }
                SelectorPanel.Visibility = Visibility.Collapsed;
                ROIPanel.Visibility = Visibility.Visible;
                ButtonState.Content = "<< Select another camera";
            }
            else
            {
                ROIPanel.Visibility = Visibility.Collapsed;
                if (_selectedCameraIndex > -1)
                {
                    WebCam camera = WebcamManager.GetCamera(_selectedCameraIndex);
                    Image_Viewer.RemoveImageSource(camera);
                    Image_Viewer.Clear();
                    Image_Viewer_Orig.RemoveImageSource(camera);
                }
            
                SelectorPanel.Visibility = Visibility.Visible;
                ButtonState.Content = "Configure person detector >>";
            }
            _selectedState = !_selectedState;
        }

        public override string Title
        {
            get
            {
                return "Person Detection";
            }
        }


        public int SelectedCameraIndex;
        public string SelectedCameraName;
        public string SelectedCameraPath;
        public double RoiX, RoiY, RoiWidth, RoiHeight;
        public int CameraFocus, CameraZoom;
        private bool _selectedState;
        private long _focusMin, _focusStep, _focusMax;
        private long _zoomMin, _zoomStep, _zoomMax;
        private int _selectedCameraIndex;
        private double topLeftX = 0;
        private double topLeftY = 0;
        private double bottomRightX = 0;
        private double bottomrigthY = 0;
        private bool setRect = false;
        public int CurrentZoom;
        public int CurrentFocus;

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
        }

        public double FocusMinimum
        {
            get { return Convert.ToDouble(_focusMin); }
            set
            {
                if (_focusMin != value)
                {
                    _focusMin = Convert.ToInt64(value);
                    NotifyPropertyChanged("FocusMinimum");
                }
            }
        }

        public double FocusMaximum
        {
            get { return Convert.ToDouble(_focusMax); }
            set
            {
                if (_focusMax != value)
                {
                    _focusMax = Convert.ToInt64(value);
                    NotifyPropertyChanged("FocusMaximum");
                }
            }
        }

        public double FocusStep
        {
            get { return Convert.ToDouble(_focusStep); }
            set
            {
                if (_focusStep != value)
                {
                    _focusStep = Convert.ToInt64(value);
                    NotifyPropertyChanged("FocusStep");
                }
            }
        }


        public double ZoomMinimum
        {
            get { return Convert.ToDouble(_zoomMin); }
            set
            {
                if (_zoomMin != value)
                {
                    _zoomMin = Convert.ToInt64(value);
                    NotifyPropertyChanged("ZoomMinimum");
                }
            }
        }

        public double ZoomMaximum
        {
            get { return Convert.ToDouble(_zoomMax); }
            set
            {
                if (_zoomMax != value)
                {
                    _zoomMax = Convert.ToInt64(value);
                    NotifyPropertyChanged("ZoomMaximum");
                }
            }
        }

        public double ZoomStep
        {
            get { return Convert.ToDouble(_zoomStep); }
            set
            {
                if (_zoomStep != value)
                {
                    _zoomStep = Convert.ToInt64(value);
                    NotifyPropertyChanged("ZoomStep");
                }
            }
        }

        public void Focus_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            long value = Convert.ToInt64(e.NewValue);
            CurrentFocus = Convert.ToInt32(value);
            if (_selectedCameraIndex > -1 && _selectedCameraIndex < WebcamManager.NumberConnectedCameras())
            {
                WebcamManager.SetPropertyValueManual(_selectedCameraIndex, WebcamManager.CAMERA_PROPERTY.CAMERA_FOCUS, value);
            }
        }


        public void Zoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            long value = Convert.ToInt64(e.NewValue);
            CurrentZoom = Convert.ToInt32(value);
            if (_selectedCameraIndex > -1 && _selectedCameraIndex < WebcamManager.NumberConnectedCameras())
            {
                WebcamManager.SetPropertyValueManual(_selectedCameraIndex, WebcamManager.CAMERA_PROPERTY.CAMERA_ZOOM, value);
            }
        }


        public void SetSliderValues(int cameraFocus, int cameraZoom)
        {
            Focus_Slider.Value = CurrentFocus = cameraFocus;
            Zoom_Slider.Value = CurrentZoom = cameraZoom;
        }

        public void SetROI(double roix, double roiy, double roiwidth, double roiheight)
        {

            ROI.Width = Math.Max(Math.Min(320 * roiwidth, 320), 0);
            ROI.Height = Math.Max(Math.Min(240 * roiheight, 240), 0);
            Canvas.SetLeft(ROI, Math.Max(Math.Min(320 * roix, 320), 0));
            Canvas.SetTop(ROI, Math.Max(Math.Min(240 * roiy, 240), 0));
            RoiX = Image_Viewer.RoiX = roix;
            RoiY = Image_Viewer.RoiY = roiy;
            RoiWidth = Image_Viewer.RoiWidth = roiwidth;
            RoiHeight = Image_Viewer.RoiHeight = roiheight;
        }

        public void CleanUp()
        {
            if (_selectedCameraIndex > -1)
            {
                WebCam camera = WebcamManager.GetCamera(_selectedCameraIndex);
                Image_Viewer.RemoveImageSource(camera);
                Image_Viewer.Clear();
                Image_Viewer_Orig.RemoveImageSource(camera);
            }
        }

        public void SetImageSource(int index)
        {
            if (index > -1 && _selectedCameraIndex < WebcamManager.NumberConnectedCameras())
            {
                _selectedCameraIndex = index;
                WebCam camera = WebcamManager.GetCamera(index);
                long mn = 0, mx = 0, step = 0;
                if (WebcamManager.GetMinPropertyValue(index, WebcamManager.CAMERA_PROPERTY.CAMERA_FOCUS, ref mn) && WebcamManager.GetMaxPropertyValue(index, WebcamManager.CAMERA_PROPERTY.CAMERA_FOCUS, ref mx))
                {
                    FocusMinimum = Convert.ToDouble(mn);
                    FocusMaximum = Convert.ToDouble(mx);
                    if (FocusMinimum != FocusMaximum)
                        Focus_Slider.IsEnabled = true;
                    else
                    {
                        FocusMinimum = 0;
                        FocusMaximum = 1;
                        Focus_Slider.IsEnabled = false;
                    }
                }
                else
                {
                    FocusMinimum = 0;
                    FocusMaximum = 1;
                    Focus_Slider.IsEnabled = false;
                }

                if (WebcamManager.GetMinPropertyValue(index, WebcamManager.CAMERA_PROPERTY.CAMERA_ZOOM, ref mn) && WebcamManager.GetMaxPropertyValue(index, WebcamManager.CAMERA_PROPERTY.CAMERA_ZOOM, ref mx))
                {
                    ZoomMinimum = Convert.ToDouble(mn);
                    ZoomMaximum = Convert.ToDouble(mx);
                    if (ZoomMinimum != ZoomMaximum)
                        Zoom_Slider.IsEnabled = true;
                    else
                    {
                        ZoomMinimum = 0;
                        ZoomMaximum = 1;
                        Zoom_Slider.IsEnabled = false;
                    }
                }
                else
                {
                    ZoomMinimum = 0;
                    ZoomMaximum = 1;
                    Zoom_Slider.IsEnabled = false;
                }

                Image_Viewer.SetImageSource(camera);
                Image_Viewer_Orig.SetImageSource(camera);
            }
        }


        private void Original_Checked(object sender, RoutedEventArgs e)
        {
            Image_Viewer.Visibility = System.Windows.Visibility.Collapsed;
            Image_Viewer_Orig.Visibility = System.Windows.Visibility.Visible;
        }

        private void Motion_HistoryChecked(object sender, RoutedEventArgs e)
        {
            Image_Viewer.Visibility = System.Windows.Visibility.Visible;
            Image_Viewer_Orig.Visibility = System.Windows.Visibility.Collapsed;
        }

       
        private void ROI_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!setRect)
            {
                System.Windows.Point pt = e.MouseDevice.GetPosition(sender as Canvas);
                if (pt.X < 320 && pt.Y < 240)
                {
                    topLeftY = topLeftX = bottomrigthY = bottomRightX = 0;
                    setRect = true;
                    topLeftX = pt.X / 320.0; topLeftY = pt.Y / 240.0;
                }
            }
        }

        private void ROI_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (setRect == true)
            {
                //get mouse location relative to the canvas 
                System.Windows.Point pt = e.MouseDevice.GetPosition(sender as Canvas);
                if (pt.X > 320 || pt.Y > 240 || pt.X / 320.0 < topLeftX || pt.Y / 240.0 < topLeftY)
                {
                    topLeftX = 0;
                    topLeftY = 0;
                    bottomrigthY = 1;
                    bottomRightX = 1;
                    ROI.Width = 320;
                    ROI.Height = 240;
                    Canvas.SetLeft(ROI, 0);
                    Canvas.SetTop(ROI, 0);
                    RoiX = Image_Viewer.RoiX = 0;
                    RoiY = Image_Viewer.RoiY = 0;
                    RoiWidth = Image_Viewer.RoiWidth = 1;
                    RoiHeight = Image_Viewer.RoiHeight = 1;
                    setRect = false;

                    return;
                }
                Canvas.SetLeft(ROI, 320 * topLeftX);
                Canvas.SetTop(ROI, 240 * topLeftY);
                ROI.Width = System.Math.Abs((int)320 * (pt.X / 320.0 - topLeftX));
                ROI.Height = System.Math.Abs((int)240 * (pt.Y / 240.0 - topLeftY));
            }
        }

        private void ROI_MouseUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point pt = e.MouseDevice.GetPosition(sender as Canvas);

            if (setRect == true && pt.X <= 320 && pt.Y <= 240 && pt.X / 320.0 > topLeftX && pt.Y / 240.0 > topLeftY)
            {
                bottomRightX = pt.X / 320;
                bottomrigthY = pt.Y / 240;
                ROI.Width = System.Math.Abs((int)320 * (bottomRightX - topLeftX));
                ROI.Height = System.Math.Abs((int)240 * (bottomrigthY - topLeftY));
                if (ROI.Width == 0 || ROI.Height == 0)
                {
                    topLeftX = 0;
                    topLeftY = 0;
                    bottomrigthY = 1;
                    bottomRightX = 1;
                    ROI.Width = 320;
                    ROI.Height = 240;
                    Canvas.SetLeft(ROI, 0);
                    Canvas.SetTop(ROI, 0);
                    RoiX = Image_Viewer.RoiX = 0;
                    RoiY = Image_Viewer.RoiY = 0;
                    RoiWidth = Image_Viewer.RoiWidth = 1;
                    RoiHeight = Image_Viewer.RoiHeight = 1;
                    setRect = false;

                    return;

                }
                Canvas.SetLeft(ROI, 320 * topLeftX);
                Canvas.SetTop(ROI, 240 * topLeftY);
                RoiX = Image_Viewer.RoiX = topLeftX;
                RoiY = Image_Viewer.RoiY = topLeftY;
                RoiWidth = Image_Viewer.RoiWidth = ROI.Width / 320.0;
                RoiHeight = Image_Viewer.RoiHeight = ROI.Height / 240.0;
                setRect = false;
            }
        } 

    }
}
