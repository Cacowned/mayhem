/* CamSnapshotConfig.xaml.cs
 * 
 * Snapshot configuration window using OpenCV Camera Library
 * 
 * Authors: Sven Kratz, Eli White
 * (c) 2011 Mayhem Open Source Project by Microsoft inc. 
 * 
 * 
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MayhemCore;
using MayhemOpenCVWrapper;
using MayhemWpf.UserControls;
using Brushes = System.Windows.Media.Brushes;
namespace VisionModules.Wpf
{

    /// <summary>
    /// Interaction logic for WebcamSnapshotConfig.xaml
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

        int selectedDeviceIdx_;
        public int SelectedDeviceIdx
        {
            get {return  camera_selector.SelectedIndex;}
            set { selectedDeviceIdx_ = value; }
        }

        // the selected camera
        private Camera camera_selected_ = null;
        public Camera camera_selected
        {
            get 
            { 
                int index = SelectedDeviceIdx;
                return CameraDriver.Instance.CamerasAvailable[index];
            }
        }

        public Camera selected_camera = null;

        private CameraDriver i = CameraDriver.Instance;
        protected List<Camera> cams = new List<Camera>();

        public ObservableCollection<ImageBrush> camera_previews = new ObservableCollection<ImageBrush>();

        // list of borders

        private List<Border> borders = new List<Border>();


        private delegate void ImageUpdateHandler(Camera c);


        // binding to the slider value 
        public double slider_value;

        public PictureConfig(string location, string prefix,  double capture_offset_time, int deviceIdx)
        {
            this.SaveLocation = location;
            FilenamePrefix = prefix;
            // directory and prefix boxes
           
            slider_value = capture_offset_time;
            SelectedDeviceIdx = deviceIdx;
            
            InitializeComponent();
             
        }

        public override void OnLoad()
        {
            InitCameraSelector();

            // populate device list
            this.box_current_loc.Text = SaveLocation;
            this.box_filename_prefix.Text = FilenamePrefix;

            CheckValidity();

            Logger.WriteLine("Nr of Cameras available: " + i.DeviceCount);
        
            // capture offset slider
            int capture_size_s = Camera.LoopDuration / 1000;
            slider_capture_offset.Minimum = -capture_size_s;
            slider_capture_offset.Maximum = capture_size_s;
            slider_capture_offset.IsDirectionReversed = false;
            slider_capture_offset.IsMoveToPointEnabled = true;
            slider_capture_offset.AutoToolTipPrecision = 2;
            slider_capture_offset.AutoToolTipPlacement =
              AutoToolTipPlacement.BottomRight;
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
            slider_capture_offset.Value = slider_value;
            //          
            CanSave = true;
        }

        private void InitCameraSelector()
        {
            camera_selector.Init();
            camera_selector.OnCameraSelected += new MultiCameraSelector.CameraSelectedHandler(
                    (Camera c) =>
                    {
                        Logger.WriteLine("Handling OnCameraSelected");
                        camera_selected_ = c;
                    }
                    );
            if (camera_selector.camera_previews.Count > 0)
                this.CanSave = true;

            // evil hack to get the camera selector to actually show the selection!
            System.Timers.Timer tt = new System.Timers.Timer(250);
            tt.AutoReset = false;
            tt.Elapsed += new System.Timers.ElapsedEventHandler((object o, System.Timers.ElapsedEventArgs e) =>
            {
                Logger.WriteLine("Timer Callback");
                Dispatcher.Invoke((Action)(() =>
                {
                    camera_selector.deviceList.SelectedIndex = SelectedDeviceIdx;
                    camera_selector.deviceList_SelectionChanged(this, null);
                }));

            });
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

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
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
            slider_value = slider_capture_offset.Value;
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
