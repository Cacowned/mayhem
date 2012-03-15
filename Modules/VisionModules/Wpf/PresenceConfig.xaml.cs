using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using MayhemCore;
using MayhemOpenCVWrapper;
using MayhemWpf.UserControls;
using VisionModules.Events;

namespace VisionModules.Wpf
{
    /// <summary>
    /// Interaction logic for PresenceConfig.xaml
    /// </summary>
    public partial class PresenceConfig : WpfConfiguration
    {
        public int SelectedIndex { get; set; }

        /// <summary>
        /// Sensitivity Slider 
        /// </summary>
        private int sliderValue;
        public double SelectedSensitivity
        {
            get
            {
                // map slider (0..100) to a (0.005 -- 0.1) range
                return sliderValue * (0.0095) / 100 + 0.005;
            }
        }

        // the selected camera
        public Camera CameraSelected
        {
            get;
            private set;
        }

        // the selected trigger mode of the event
        public PresenceTriggerMode selected_triggerMode
        {
            get;
            private set;
        }

        public PresenceConfig(int selectedCameraIndex, PresenceTriggerMode selectedTriggerMode, int aSlidervalue)
        {
            selected_triggerMode = PresenceTriggerMode.Toggle;
            sliderValue = 20;
            SelectedIndex = selectedCameraIndex;

            sliderValue = aSlidervalue;

            InitializeComponent();

            selected_triggerMode = selectedTriggerMode;

            // set the previous trigger mode
            switch (selectedTriggerMode)
            {
                case PresenceTriggerMode.Toggle:
                    rb_triggerMode_toggle.IsChecked = true;
                    break;
                case PresenceTriggerMode.OnOff:
                    rb_triggerMode_onOff.IsChecked = true;
                    break;
                case PresenceTriggerMode.OffOn:
                    rb_triggerMode_offOn.IsChecked = true;
                    break;
            }
        }

        public override void OnLoad()
        {
            InitCameraSelector();

            sdr_sensitivity.Minimum = 0;
            sdr_sensitivity.Maximum = 100;
            sdr_sensitivity.IsDirectionReversed = false;
            sdr_sensitivity.IsMoveToPointEnabled = true;
            sdr_sensitivity.AutoToolTipPrecision = 2;
            sdr_sensitivity.AutoToolTipPlacement = AutoToolTipPlacement.BottomRight;
            sdr_sensitivity.TickPlacement = TickPlacement.TopLeft;
            sdr_sensitivity.TickFrequency = 5;
            sdr_sensitivity.IsSelectionRangeEnabled = true;
            sdr_sensitivity.SelectionStart = 0;
            sdr_sensitivity.SelectionEnd = 100;
            sdr_sensitivity.SmallChange = 1;
            sdr_sensitivity.LargeChange = 5;
            sdr_sensitivity.Value = sliderValue;
        }

        private void InitCameraSelector()
        {
            camera_selector.Init();
            camera_selector.OnCameraSelected += c =>
                                                    {
                                                        Logger.WriteLine("Handling OnCameraSelected");
                                                        CameraSelected = c;
                                                    };
            if (camera_selector.CameraPreviews.Count > 0)
                CanSave = true;

            // evil hack to get the camera selector to actually show the selection!
            System.Timers.Timer tt = new System.Timers.Timer(250);
            tt.AutoReset = false;
            tt.Elapsed += (o, e) =>
                              {
                                  Logger.WriteLine("Timer Callback");
                                  Dispatcher.Invoke((Action)(() =>
                                                                 {
                                                                     camera_selector.deviceList.SelectedIndex = SelectedIndex;
                                                                     camera_selector.deviceList_SelectionChanged(this, null);
                                                                 }));

                              };
            tt.Enabled = true;

            camera_selector.deviceList.SelectedIndex = SelectedIndex;
        }

        public override void OnSave()
        {
            CameraSelected = camera_selector.SelectedCamera;
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
                selected_triggerMode = PresenceTriggerMode.Toggle;
            }
            else if (rb == rb_triggerMode_offOn)
            {
                selected_triggerMode = PresenceTriggerMode.OffOn;
            }
            else if (rb == rb_triggerMode_onOff)
            {
                selected_triggerMode = PresenceTriggerMode.OnOff;
            }
        }

        private void sdr_sensitivity_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            sliderValue = (int)sdr_sensitivity.Value;
        }
    }
}
