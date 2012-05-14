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
    public partial class WebCamMotionDetectionConfig : WpfConfiguration, INotifyPropertyChanged
    {
    
        public WebCamMotionDetectionConfig()
        {
            InitializeComponent();
            camera_selector.InitSelector();
            SelectedCameraIndex = camera_selector.GetSelectedCameraIndex();
            camera_roi_selector.SetImageSource(1);
        }

        public override void OnClosing()
        {
            camera_roi_selector.CleanUp();
            camera_selector.Cleanup();
        }

        
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
            {
                System.Windows.MessageBox.Show("Changed = " + SelectedCameraIndex.ToString());
                PropertyChanged(this, new PropertyChangedEventArgs(property));
                if (property == "SelectedCameraIndex")
                {
                    if (SelectedCameraIndex > -1)
                    {
                        camera_roi_selector.SetImageSource(SelectedCameraIndex);
                    }
                }
            }
        }

        public int SelectedCameraIndex
        {
            get { return _selectedCameraIndex; }
            set
            {
                if (_selectedCameraIndex != value)
                {
                    _selectedCameraIndex = Convert.ToInt32(value);
                    NotifyPropertyChanged("SelectedCameraIndex");
                }
            }
        }


        private int _selectedCameraIndex;
    }
}
