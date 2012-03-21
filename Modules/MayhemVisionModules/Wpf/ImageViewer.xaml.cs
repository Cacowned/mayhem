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
    /// Interaction logic for ImageViewer.xaml
    /// </summary>
    /// /// <summary>
    /// A simple viewer class that encapsulates ImageListenerBase into a view target that includes a border, a box showing framerate and a textbox to display device name
    /// </summary>
    public partial class ImageViewer : UserControl, INotifyPropertyChanged
    {
        public ImageViewer()
        {
            InitializeComponent();
            ViewerWidth = 640;
            ViewerHeight = 480;
            ViewerOrientation = 0;
            ViewerScaleX = 1;
            ViewerScaleY = 1;
            ViewerSkewX = 0;
            ViewerSkewY = 0;
            ViewerTranslateX = 0;
            ViewerTranslateY = 100;
            ViewerUpdated = false;
        }

        public void AddImageSource(ImagerBase c)
        {
            if (ImageSource.SubscribedImagers.Count > 0)
            {
                RemoveImageSource(ImageSource.SubscribedImagers[0]);
            }
            ImageSource.RegisterForImages(c);
            ViewerUpdated = true;
        }

        public void RemoveImageSource(ImagerBase c)
        {
            ImageSource.UnregisterForImages(c);
            ViewerUpdated = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            if (property == "ViewerWidth" || property == "ViewerHeight" || property == "ViewerUpdate")
            {
                //scale image accordingly
                double scaleX = ViewerWidth / ImageSource.ImagerWidth;
                double scaleY = ViewerHeight / ImageSource.ImagerHeight;
                ViewerScaleX = double.IsNaN(scaleX) ? 1 : (scaleX == 0) ? 1 : scaleX;
                ViewerScaleY = double.IsNaN(scaleY) ? 1 : (scaleY == 0) ? 1 : scaleY;
            }
        }

        public bool ViewerUpdated
        {
            get { return _viewUpdate; }
            set { _viewUpdate = (bool)value; NotifyPropertyChanged("ViewerUpdate"); }
        }

        public double ViewerWidth
        {
            get { return _viewWidth; }
            set
            {
                if (_viewWidth != value)
                {
                    _viewWidth = Convert.ToDouble(value);
                    NotifyPropertyChanged("ViewerWidth");
                }
            }
        }

        public double ViewerHeight
        {
            get { return _viewHeight; }
            set
            {
                if (_viewHeight != value)
                {
                    _viewHeight = Convert.ToDouble(value);
                    NotifyPropertyChanged("ViewerHeight");
                }
            }
        }

        public double ViewerTranslateX
        {
            get { return _viewTranslateX; }
            set
            {
                if (_viewTranslateX != value)
                {
                    _viewTranslateX = Convert.ToDouble(value);
                    NotifyPropertyChanged("ViewerTranslateX");
                }
            }
        }
        public double ViewerTranslateY
        {
            get { return _viewTranslateY; }
            set
            {
                if (_viewTranslateY != value)
                {
                    _viewTranslateY = Convert.ToDouble(value);
                    NotifyPropertyChanged("ViewerTranslateY");
                }
            }
        }
        public double ViewerOrientation
        {
            get { return _viewOrientation; }
            set 
            {
                if (_viewOrientation != value)
                {
                    _viewOrientation = Convert.ToDouble(value);
                    NotifyPropertyChanged("RenderOrientation");
                }
            }
        }
        public double ViewerScaleX
        {
            get { return _viewScaleX; }
            set
            {
                if (_viewScaleX != value)
                {
                    _viewScaleX = Convert.ToDouble(value);
                    NotifyPropertyChanged("ViewerScaleX");
                }
            }
        }
        public double ViewerScaleY
        {
            get { return _viewScaleY; }
            set
            {
                if (_viewScaleY != value)
                {
                    _viewScaleY = Convert.ToDouble(value);
                    NotifyPropertyChanged("ViewerScaleY");
                }
            }
        }
        public double ViewerSkewX
        {
            get { return _viewSkewX; }
            set
            {
                if (_viewSkewX != value)
                {
                    _viewSkewX = Convert.ToDouble(value);
                    NotifyPropertyChanged("ViewerSkewX");
                }
            }
        }
        public double ViewerSkewY
        {
            get { return _viewSkewY; }
            set
            {
                if (_viewSkewY != value)
                {
                    _viewSkewY = Convert.ToDouble(value);
                    NotifyPropertyChanged("ViewerSkewY");
                }
            }
        }

        private bool _viewUpdate;
        private double _viewHeight;
        private double _viewWidth;
        private double _viewOrientation;
        private double _viewScaleX;
        private double _viewScaleY;
        private double _viewSkewX;
        private double _viewSkewY;
        private double _viewTranslateX;
        private double _viewTranslateY;
    }
}
