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
using MayhemWebCamWrapper;
using System.ComponentModel;
using MayhemVisionModules.Components;

namespace MayhemVisionModules.Wpf
{
    /// <summary>
    /// Interaction logic for WebCamROISelector.xaml
    /// </summary>
    public partial class WebCamROISelector : UserControl, INotifyPropertyChanged
    {
        public WebCamROISelector()
        {
            InitializeComponent();
            FocusMinimum = 0;
            FocusMaximum = 1;
            FocusStep = 1;
            ZoomMinimum = 0;
            ZoomMaximum = 1;
            ZoomStep = 1;
            CurrentPercentage = 5;
            CurrentDifference = 30;
            CurrentTime = 1;
            CurrentZoom = 1;
            CurrentFocus = 0;
            Percentage_Slider.Value = CurrentPercentage;
            Difference_Slider.Value = CurrentDifference;
            Time_Slider.Value = CurrentTime;
            _selectedCameraIndex = -1;
  
        }

        

        public void CleanUp()
        {
            if (_selectedCameraIndex > -1)
            {
                WebCam camera = WebcamManager.GetCamera(_selectedCameraIndex);
                Image_Viewer.RemoveImageSource(camera);
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


        private void ROI_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!setRect)
            {
                System.Windows.Point pt = e.MouseDevice.GetPosition(sender as Canvas);
                if (pt.X < 320 && pt.Y < 240)
                {
                    topLeftY = topLeftX = bottomrigthY = bottomRightX = 0;
                    setRect = true;
                    topLeftX = pt.X; topLeftY = pt.Y;
                }
            }
        }

        private void ROI_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (setRect == true)
            {
                //get mouse location relative to the canvas 
                System.Windows.Point pt = e.MouseDevice.GetPosition(sender as Canvas);
                if (pt.X > 320 || pt.Y > 240 || pt.X < topLeftX || pt.Y < topLeftY)
                {
                    topLeftX = 0;
                    topLeftY = 0;
                    bottomrigthY = 240;
                    bottomRightX = 320;
                    ROI.Width = System.Math.Abs((int)(bottomRightX - topLeftX));
                    ROI.Height = System.Math.Abs((int)(bottomrigthY - topLeftY));
                    Canvas.SetLeft(ROI, topLeftX);
                    Canvas.SetTop(ROI, topLeftY);
                    RoiX = Image_Viewer.RoiX = Convert.ToInt32(2 * topLeftX);
                    RoiY = Image_Viewer.RoiY = Convert.ToInt32(2 * topLeftY);
                    RoiWidth = Image_Viewer.RoiWidth = Convert.ToInt32(2 * ROI.Width);
                    RoiHeight = Image_Viewer.RoiHeight = Convert.ToInt32(2 * ROI.Height);
                    setRect = false;

                    return;
                }
                Canvas.SetLeft(ROI, topLeftX);
                Canvas.SetTop(ROI, topLeftY);
                ROI.Width = System.Math.Abs((int)(pt.X - topLeftX));
                ROI.Height = System.Math.Abs((int)(pt.Y - topLeftY));
            }
        }

        private void ROI_MouseUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point pt = e.MouseDevice.GetPosition(sender as Canvas);
                
            if (setRect == true && pt.X <= 320 && pt.Y <= 240 && pt.X > topLeftX && pt.Y > topLeftY )
            {
                bottomRightX = pt.X;
                bottomrigthY = pt.Y;
                ROI.Width = System.Math.Abs((int)(bottomRightX - topLeftX));
                ROI.Height = System.Math.Abs((int)(bottomrigthY - topLeftY));
                if (ROI.Width == 0 || ROI.Height == 0)
                {
                    topLeftX = 0;
                    topLeftY = 0;
                    bottomrigthY = 240;
                    bottomRightX = 320;
                    ROI.Width = System.Math.Abs((int)(bottomRightX - topLeftX));
                    ROI.Height = System.Math.Abs((int)(bottomrigthY - topLeftY));
                    Canvas.SetLeft(ROI, topLeftX);
                    Canvas.SetTop(ROI, topLeftY);
                    RoiX = Image_Viewer.RoiX = Convert.ToInt32(2 * topLeftX);
                    RoiY = Image_Viewer.RoiY = Convert.ToInt32(2 * topLeftY);
                    RoiWidth = Image_Viewer.RoiWidth = Convert.ToInt32(2 * ROI.Width);
                    RoiHeight = Image_Viewer.RoiHeight = Convert.ToInt32(2 * ROI.Height);
                    setRect = false;

                    return;
 
                }
                Canvas.SetLeft(ROI, topLeftX);
                Canvas.SetTop(ROI, topLeftY);
                RoiX = Image_Viewer.RoiX = Convert.ToInt32(2 * topLeftX);
                RoiY = Image_Viewer.RoiY = Convert.ToInt32(2 * topLeftY);
                RoiWidth = Image_Viewer.RoiWidth = Convert.ToInt32(2*ROI.Width);
                RoiHeight = Image_Viewer.RoiHeight = Convert.ToInt32(2 * ROI.Height);
                setRect = false;
            }
        } 



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

        private int _selectedCameraIndex;
        private long _focusMin, _focusStep, _focusMax;
        private long _zoomMin, _zoomStep, _zoomMax;
        private double topLeftX = 0;
        private double topLeftY = 0;
        private double bottomRightX = 0;
        private double bottomrigthY = 0;
        private bool setRect = false;
        public double CurrentPercentage;
        public double CurrentDifference;
        public double CurrentTime;
        public int CurrentZoom;
        public int CurrentFocus;
        public int RoiX, RoiY, RoiWidth, RoiHeight;

        public void SetSliderValues(int cameraFocus, int cameraZoom, float percentageThresh, float timeThresh, int differenceThresh)
        {
            Focus_Slider.Value = CurrentFocus  = cameraFocus;
            Zoom_Slider.Value = CurrentZoom  = cameraZoom;
            Percentage_Slider.Value = CurrentPercentage = Image_Viewer.MotionAreaPercentageSensitivity = percentageThresh;
            Time_Slider.Value = CurrentTime = Image_Viewer.TimeSensitivity = timeThresh;
            Difference_Slider.Value = CurrentDifference = Image_Viewer.MotionDiffSensitivity = differenceThresh;
        }

        public void SetROI(int roix, int roiy, int roiwidth, int roiheight)
        {

            ROI.Width = 0.5 * (roiwidth) - 0.5;
            ROI.Height = 0.5 * (roiheight) - 0.5;
            Canvas.SetLeft(ROI, 0.5*(roix)-0.5);
            Canvas.SetTop(ROI, 0.5 * (roiy) - 0.5);
            RoiX = Image_Viewer.RoiX = roix;
            RoiY = Image_Viewer.RoiY = roiy;
            RoiWidth = Image_Viewer.RoiWidth = roiwidth;
            RoiHeight = Image_Viewer.RoiHeight = roiheight;
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

        private void Percentage_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            CurrentPercentage = e.NewValue;
            if (_selectedCameraIndex > -1 && _selectedCameraIndex < WebcamManager.NumberConnectedCameras())
            {
                Image_Viewer.MotionAreaPercentageSensitivity = (float)CurrentPercentage;
            }
        }

        private void Difference_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            CurrentDifference = e.NewValue;
            if (_selectedCameraIndex > -1 && _selectedCameraIndex < WebcamManager.NumberConnectedCameras())
            {
                Image_Viewer.MotionDiffSensitivity = (int)CurrentDifference;
            }
        }

        private void Time_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            CurrentTime = e.NewValue;
            if (_selectedCameraIndex > -1 && _selectedCameraIndex < WebcamManager.NumberConnectedCameras())
            {
                Image_Viewer.TimeSensitivity = (float)CurrentTime;
            }
        }
    }
}
