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
using MayhemDefaultStyles.UserControls;
using System.Diagnostics;
using MayhemOpenCVWrapper;
using VisionModules.Events;
using MayhemCore;

namespace VisionModules.Wpf
{
    /// <summary>
    /// Interaction logic for PresenceConfig.xaml
    /// </summary>
    public partial class PresenceConfig : IWpfConfiguration
    {
        // the selected camera
        private Camera camera_selected_ = null;  
        public Camera camera_selected
        {
            get { return camera_selected_; }
        }

        // the selected trigger mode of the event
        private PresenceTriggerMode selected_triggerMode_ = PresenceTriggerMode.TOGGLE;
        public PresenceTriggerMode selected_triggerMode
        {
            get { return selected_triggerMode_; }
        }


        public PresenceConfig()
        {
            InitializeComponent();
            Init();
        }

        private void Init()
        {
            camera_selector.OnCameraSelected += new MultiCameraSelector.CameraSelectedHandler(                
                    (Camera c) => 
                        {
                            Logger.WriteLine("Handling OnCameraSelected");
                            camera_selected_ = c; 
                        }
                    );
        }

        

        private void Control_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Logger.WriteLine("IsVisibleChanged");
            camera_selector.VisibilityChanged(this.IsVisible);
        }

        private void IWpfConfiguration_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            camera_selector.VisibilityChanged(this.IsVisible);
        }

        public override void OnClosing()
        {
            base.OnClosing();
            camera_selector.OnClosing();
        }

        public override string Title
        {
            get
            {
                return "Presence Detector";
            }
        }

        private void rb_triggerMode_toggle_Checked(object sender, RoutedEventArgs e)
        {
            Logger.WriteLine("rb_triggerMode_toggle_Checked");

            RadioButton rb = sender as RadioButton;

            if (rb == rb_triggerMode_toggle)
            {
                selected_triggerMode_ = PresenceTriggerMode.TOGGLE;
            }
            else if (rb == rb_triggerMode_offOn)
            {
                selected_triggerMode_ = PresenceTriggerMode.OFF_ON;
            }
            else if (rb == rb_triggerMode_onOff)
            {
                selected_triggerMode_ = PresenceTriggerMode.ON_OFF;
            }
        }
        
    }
}
