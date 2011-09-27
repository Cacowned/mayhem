using System.IO;
using System.Windows;
using System.Windows.Controls;
using MayhemWpf.UserControls;
using Microsoft.Win32;
using System.Windows.Forms;
using System;

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
            this.SaveLocation = directory;
            this.FilenamePrefix = filenamePrefix;

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
            if(!(textBoxDirectory.Text.Length > 0 && Directory.Exists(textBoxDirectory.Text)))
            {
                textInvalid.Text = "Invalid save location";
                CanSave = false;
            }
            else if(textBoxPrefix.Text.Length == 0)
            {
                textInvalid.Text = "You must enter a filename prefix";
                CanSave = false;
            }
            else if(textBoxPrefix.Text.Length > 20)
            {
                textInvalid.Text = "Filename prefix is too long";
                CanSave = false;
            }
            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }

        // Browse for file
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.RootFolder = Environment.SpecialFolder.MyPictures;
            dlg.ShowNewFolderButton = true;
            dlg.SelectedPath = SaveLocation;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SaveLocation = dlg.SelectedPath;
                textBoxDirectory.Text = SaveLocation;
            }
        }

        private void textBoxDirectory_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }

        private void textBoxPrefix_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
