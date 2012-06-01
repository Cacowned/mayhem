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
using MayhemWebCamWrapper;

namespace MayhemVisionModules.Wpf
{
    /// <summary>
    /// Interaction logic for WebcamSnapshot.xaml
    /// </summary>
    public partial class WebcamSnapshotConfig : WpfConfiguration
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

        public string SelectedCameraPath
        {
            get { return camera_selector.GetSelectedCameraPath();}
            set { selectedCameraPath = value; }
        }

        public string SelectedCameraName
        {
            get { return camera_selector.GetSelectedCameraName(); }
            set { selectedCameraName = value; }
        }

        public int SelectedCameraIndex
        {
            get { return camera_selector.GetSelectedCameraIndex(); }
        }

        public bool ShowPreview
        {
            get;
            private set;
        }

        public bool PlayShutterSound
        {
            get;
            private set;
        }

        private string selectedCameraPath;
        private string selectedCameraName;

        public WebcamSnapshotConfig()
        {
            InitializeComponent();
            camera_selector.CamerasReady += OnSelectorCamerasReady;
            camera_selector.CamerasNotReady += OnSelectorCamerasNotReady;
            camera_selector.InitSelector();
        }
       
        public WebcamSnapshotConfig(string folderLocation, string fileNamePrefix, bool showPreview, bool playShutterSound)
        {
            InitializeComponent();

            SaveLocation = textBox_SaveLocation.Text = folderLocation;
            FilenamePrefix = textBox_fileNamePrefix.Text = fileNamePrefix;
            SelectedCameraPath = selectedCameraPath;
            ShowPreview = showPreview;
            PlayShutterSound = playShutterSound;
            chkBoxShowPreview.IsChecked = showPreview;
            chkBoxPlayShutterSound.IsChecked = playShutterSound;
            camera_selector.CamerasReady += OnSelectorCamerasReady;
            camera_selector.CamerasNotReady += OnSelectorCamerasNotReady;
            camera_selector.InitSelector();
        }

        public override void OnLoad()
        {
            textBox_SaveLocation.Text = SaveLocation;
            textBox_fileNamePrefix.Text = FilenamePrefix;
            chkBoxPlayShutterSound.IsChecked = PlayShutterSound;
            chkBoxShowPreview.IsChecked = ShowPreview;
        }

        private void OnSelectorCamerasReady(object o, EventArgs a)
        {
            CheckValidity();
        }

        private void OnSelectorCamerasNotReady(object o, EventArgs a)
        {
            CanSave = false;
        }

        public override string Title
        {
            get
            {
                return "Webcam Snapshot";
            }
        }

        private void CheckValidity()
        {
            CanSave = true;
            if (!(textBox_SaveLocation.Text.Length > 0 && Directory.Exists(textBox_SaveLocation.Text)))
            {
                if (textInvalid != null)
                    textInvalid.Text = "Invalid save location";
                CanSave = false;
            }
            else if (textBox_SaveLocation.Text.Length == 0)
            {
                if (textInvalid != null)
                    textInvalid.Text = "You must enter a filename prefix";
                CanSave = false;
            }
            else if (textBox_fileNamePrefix.Text.Length > 100)
            {
                if (textInvalid != null)
                    textInvalid.Text = "Filename prefix is too long";
                CanSave = false;
            }
            else if (!HasAccessToWrite(textBox_SaveLocation.Text))
            {
                if (textInvalid != null)
                    textInvalid.Text = "No write access to selected directory";
                CanSave = false; 
            }
            if (textInvalid != null)
            {
                textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private bool HasAccessToWrite(string path)
        {
            try
            {
                using (FileStream fs = File.Create(System.IO.Path.Combine(path, "Access.txt"), 1, FileOptions.DeleteOnClose))
                {
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public override void OnSave()
        {
            SaveLocation = textBox_SaveLocation.Text;
            FilenamePrefix = textBox_fileNamePrefix.Text;
            SelectedCameraPath = camera_selector.GetSelectedCameraPath();
            ShowPreview = chkBoxShowPreview.IsChecked.HasValue && chkBoxShowPreview.IsChecked.Value;
            PlayShutterSound = chkBoxPlayShutterSound.IsChecked.HasValue && chkBoxPlayShutterSound.IsChecked.Value;
        }

        public override void OnClosing()
        {
            camera_selector.Cleanup();
        }

        private void TextBoxSaveLocationChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();
            if (textInvalid != null)
                textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }

        private void TextBoxFilenamePrefix_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();
            if (textInvalid != null)
                textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }

        private void ButtonSaveLocationClick(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.RootFolder = Environment.SpecialFolder.MyComputer;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                SaveLocation = dlg.SelectedPath;
                textBox_SaveLocation.Text = SaveLocation;
            }
        }

        private void TextBoxCaptureWidth_Changed(object sender, TextChangedEventArgs e)
        {
            CheckValidity();
            if (textInvalid != null)
                textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }

        private void TextBoxCaptureHeight_Changed(object sender, TextChangedEventArgs e)
        {
            CheckValidity();
            if (textInvalid != null)
                textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
