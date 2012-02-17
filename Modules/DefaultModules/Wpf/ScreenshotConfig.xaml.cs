using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using MayhemWpf.UserControls;

namespace DefaultModules.Wpf
{
    public partial class ScreenshotConfig : WpfConfiguration
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

        public ScreenshotConfig(string directory, string filenamePrefix)
        {
            SaveLocation = directory;
            FilenamePrefix = filenamePrefix;

            InitializeComponent();
        }

        public override string Title
        {
            get { return "Screenshot"; }
        }

        public override void OnLoad()
        {
            textBoxDirectory.Text = SaveLocation;
            textBoxPrefix.Text = FilenamePrefix;
        }

        public override void OnSave()
        {
            SaveLocation = textBoxDirectory.Text;
            FilenamePrefix = textBoxPrefix.Text;
        }

        private void CheckValidity()
        {
            CanSave = true;
            if (!(textBoxDirectory.Text.Length > 0 && Directory.Exists(textBoxDirectory.Text)))
            {
                textInvalid.Text = "Invalid save location";
                CanSave = false;
            }
            else if (textBoxPrefix.Text.Length == 0)
            {
                textInvalid.Text = "You must enter a filename prefix";
                CanSave = false;
            }
            else if (textBoxPrefix.Text.Length > 20)
            {
                textInvalid.Text = "Filename prefix is too long";
                CanSave = false;
            }

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }

        // Browse for file
        private void Browse_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new FolderBrowserDialog();

            dlg.RootFolder = Environment.SpecialFolder.MyPictures;
            dlg.ShowNewFolderButton = true;
            dlg.SelectedPath = SaveLocation;

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                SaveLocation = dlg.SelectedPath;
                textBoxDirectory.Text = SaveLocation;
            }
        }

        private void Directory_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }

        private void Prefix_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
