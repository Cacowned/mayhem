using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;
using System.Windows.Media;
using MayhemCore;
using MayhemOpenCVWrapper;
using MayhemWpf.UserControls;
namespace VisionModules.Wpf
{

    /// <summary>
    /// Snapshot configuration window using OpenCV Camera Library
    /// </summary>
    public partial class PictureConfig : WpfConfiguration
    {
        public string SaveLocation
        {
            get;
            private set;
        }

        public string FilenamePrefix
        {
            get;
            private set;
        }

        private int selectedDeviceIdx_;
        public int SelectedDeviceIdx
        {
            get { return camera_selector.SelectedIndex; }
            set { selectedDeviceIdx_ = value; }
        }

        // the selected camera
        private Camera camera_selected_;
        public Camera CameraSelected
        {
            get
            {
                int index = SelectedDeviceIdx;
                return CameraDriver.Instance.CamerasAvailable[index];
            }
            private set
            {
                camera_selected_ = value;
            }
        }

        // binding to the slider value (needs to stay public due to binding
        public double SliderValue { get; set; }

        public Camera SelectedCamera;

        private CameraDriver i;

        public PictureConfig(string location, string prefix, double captureOffsetTime, int deviceIdx)
        {
            i = CameraDriver.Instance;
           
            // directory and prefix boxes
            SaveLocation = location;
            FilenamePrefix = prefix;

            SliderValue = captureOffsetTime;
            SelectedDeviceIdx = deviceIdx;
            InitializeComponent();
        }

        public override void OnLoad()
        {
            InitCameraSelector();

            // populate device list
            box_current_loc.Text = SaveLocation;
            box_filename_prefix.Text = FilenamePrefix;

            CheckValidity();

            Logger.WriteLine("Nr of Cameras available: " + i.DeviceCount);

            // capture offset slider
            int capture_size_s = Camera.LoopDuration / 1000;
            slider_capture_offset.Minimum = -capture_size_s;
            slider_capture_offset.Maximum = capture_size_s;
            slider_capture_offset.IsDirectionReversed = false;
            slider_capture_offset.IsMoveToPointEnabled = true;
            slider_capture_offset.AutoToolTipPrecision = 2;
            slider_capture_offset.AutoToolTipPlacement = AutoToolTipPlacement.BottomRight;
            slider_capture_offset.TickPlacement = TickPlacement.TopLeft;
            slider_capture_offset.TickFrequency = 0.5;
            DoubleCollection tickMarks = new DoubleCollection();
            for (int k = -capture_size_s; k <= capture_size_s; k++)
            {
                tickMarks.Add(k);
            }
            slider_capture_offset.Ticks = tickMarks;
            slider_capture_offset.IsSelectionRangeEnabled = true;
            slider_capture_offset.SelectionStart = -capture_size_s;
            slider_capture_offset.SelectionEnd = capture_size_s;
            slider_capture_offset.SmallChange = 0.5;
            slider_capture_offset.LargeChange = 1.0;
            slider_capture_offset.Value = SliderValue;
          
            CanSave = true;
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
                    camera_selector.deviceList.SelectedIndex = SelectedDeviceIdx;
                    camera_selector.deviceList_SelectionChanged(this, null);
                }));
            };
            tt.Enabled = true;

            camera_selector.deviceList.SelectedIndex = SelectedDeviceIdx;
        }

        /// <summary>
        /// Shows the dialog to select the file name of the template image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.RootFolder = Environment.SpecialFolder.MyComputer;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                SaveLocation = dlg.SelectedPath;
                box_current_loc.Text = SaveLocation;
            }
        }

        public override string Title
        {
            get
            {
                return "Picture";
            }
        }

        private void CheckValidity()
        {
            CanSave = true;
            if (!(box_current_loc.Text.Length > 0 && Directory.Exists(box_current_loc.Text)))
            {
                if (textInvalid != null)
                    textInvalid.Text = "Invalid save location";
                CanSave = false;
            }
            else if (box_current_loc.Text.Length == 0)
            {
                if (textInvalid != null)
                    textInvalid.Text = "You must enter a filename prefix";
                CanSave = false;
            }
            else if (box_current_loc.Text.Length > 100)
            {
                if (textInvalid != null)
                    textInvalid.Text = "Filename prefix is too long";
                CanSave = false;
            }
            if (textInvalid != null)
                textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }

        public override void OnSave()
        {
            SaveLocation = box_current_loc.Text;
            FilenamePrefix = box_filename_prefix.Text;
        }

        private void slider_capture_offset_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            SliderValue = slider_capture_offset.Value;
        }

        private void lbl_current_loc_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();
            if (textInvalid != null)
                textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }

        private void box_filename_prefix_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();
            if (textInvalid != null)
                textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
