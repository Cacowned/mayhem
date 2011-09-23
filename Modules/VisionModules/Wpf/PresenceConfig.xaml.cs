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
using System.Diagnostics;
using MayhemOpenCVWrapper;
using VisionModules.Events;
using MayhemCore;
using System.Threading;

namespace VisionModules.Wpf
{
    /// <summary>
    /// Interaction logic for PresenceConfig.xaml
    /// </summary>
    public partial class PresenceConfig : IWpfConfiguration
    {
        public int selectedIndex = 0; 
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


        public PresenceConfig(int selectedCameraIndex, PresenceTriggerMode selectedTriggerMode)
        {
            this.selectedIndex = selectedCameraIndex; 
           
            InitializeComponent();
            
            Init();

            selected_triggerMode_ = selectedTriggerMode; 

            // set the previous trigger mode
            switch (selectedTriggerMode)
            {
                case PresenceTriggerMode.TOGGLE :
                     rb_triggerMode_toggle.IsChecked = true; 
                     break;
                case PresenceTriggerMode.ON_OFF :
                     rb_triggerMode_onOff.IsChecked = true;
                     break;
                case PresenceTriggerMode.OFF_ON :
                     rb_triggerMode_offOn.IsChecked = true;                 
                     break;                  
            }

            // evil hack to get the camera selector to actually show the selection!
            System.Timers.Timer tt = new System.Timers.Timer(250);
            tt.AutoReset = false; 
            tt.Elapsed+= new System.Timers.ElapsedEventHandler((object o, System.Timers.ElapsedEventArgs e) => {
                Logger.WriteLine("Timer Callback");
                Dispatcher.Invoke((Action)(() => { 
                    camera_selector.deviceList.SelectedIndex = this.selectedIndex;
                    camera_selector.deviceList_SelectionChanged(this, null); 
                }) );          
            });
            tt.Enabled = true;

            camera_selector.deviceList.SelectedIndex = selectedCameraIndex;
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
            if (camera_selector.camera_previews.Count > 0)
                this.CanSave = true; 
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

        public override void OnSave()
        {
            base.OnSave();
            camera_selected_ = camera_selector.selected_camera; 
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
