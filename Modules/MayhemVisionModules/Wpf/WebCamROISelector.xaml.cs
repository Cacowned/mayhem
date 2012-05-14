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
            _selectedCameraIndex = -1;
  
        }

        public void CleanUp()
        {
            if (_selectedCameraIndex > -1)
            {
                WebCam camera = WebcamManager.GetCamera(_selectedCameraIndex);
                Image_Viewer.RemoveImageSource(camera);
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
                        Focus_Slider.IsEnabled = false;
                }
                else
                    Focus_Slider.IsEnabled = false;

                if (WebcamManager.GetMinPropertyValue(index, WebcamManager.CAMERA_PROPERTY.CAMERA_ZOOM, ref mn) && WebcamManager.GetMaxPropertyValue(index, WebcamManager.CAMERA_PROPERTY.CAMERA_ZOOM, ref mx))
                {
                    ZoomMinimum = Convert.ToDouble(mn);
                    ZoomMaximum = Convert.ToDouble(mx);
                    if (ZoomMinimum != ZoomMaximum)
                        Zoom_Slider.IsEnabled = true;
                    else 
                        Zoom_Slider.IsEnabled = false;
                }
                else
                    Zoom_Slider.IsEnabled = false;

                Image_Viewer.TopTitle = camera.WebCamName;
                Image_Viewer.SetImageSource(camera);
            }
        }


        private void ROI_MouseDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point pt = e.MouseDevice.GetPosition(sender as Canvas);
            if (pt.X >= 30 && pt.Y >= 30)
            {
                topLeftY = topLeftX = bottomrigthY = bottomRightX = 0;
                setRect = true;
                topLeftX = pt.X; topLeftY = pt.Y;
            }
        }

        private void ROI_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            if (setRect == true)
            {
                //get mouse location relative to the canvas 
                System.Windows.Point pt = e.MouseDevice.GetPosition(sender as Canvas);
                Canvas.SetLeft(ROI, topLeftX);
                Canvas.SetTop(ROI, topLeftY);
                ROI.Width = System.Math.Abs((int)(pt.X - topLeftX));
                ROI.Height = System.Math.Abs((int)(pt.Y - topLeftY));
            }
        }

        private void ROI_MouseUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Point pt = e.MouseDevice.GetPosition(sender as Canvas);
            bottomRightX = pt.X;
            bottomrigthY = pt.Y;
            ROI.Width = System.Math.Abs((int)(bottomRightX - topLeftX));
            ROI.Height = System.Math.Abs((int)(bottomrigthY - topLeftY));
            Canvas.SetLeft(ROI, topLeftX);
            Canvas.SetTop(ROI, topLeftY);
            setRect = false;
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
            if (_selectedCameraIndex > -1 && _selectedCameraIndex < WebcamManager.NumberConnectedCameras())
                WebcamManager.SetPropertyValueManual(_selectedCameraIndex, WebcamManager.CAMERA_PROPERTY.CAMERA_FOCUS, value);
        }

       
        public void Zoom_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            long value = Convert.ToInt64(e.NewValue);
            if (_selectedCameraIndex > -1 && _selectedCameraIndex < WebcamManager.NumberConnectedCameras())
                WebcamManager.SetPropertyValueManual(_selectedCameraIndex, WebcamManager.CAMERA_PROPERTY.CAMERA_ZOOM, value);
        }

        private int _selectedCameraIndex;
        private long _focusMin, _focusStep, _focusMax;
        private long _zoomMin, _zoomStep, _zoomMax;
        private double topLeftX = 0;
        private double topLeftY = 0;
        private double bottomRightX = 0;
        private double bottomrigthY = 0;
        private bool setRect = false;
    }
}
