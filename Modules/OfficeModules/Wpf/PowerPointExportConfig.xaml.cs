using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using MayhemCore;
using MayhemWpf.UserControls;
using OfficeModules.Resources;

namespace OfficeModules.Wpf
{
    public partial class PowerPointExportConfig : WpfConfiguration
    {
        public string FileName
        {
            get;
            private set;
        }

        public override string Title
        {
            get { return "Export"; }
        }

        public PowerPointExportConfig(string filename)
        {
            InitializeComponent();

            FileName = filename;
        }

        public override void OnLoad()
        {
            LocationBox.Text = FileName;
            CheckValidity();
        }

        public override void OnSave()
        {
            FileName = LocationBox.Text;
        }

        private void CheckValidity()
        {
            string text = LocationBox.Text;

            CanSave = true;

            if (text.Length == 0)
            {
                textInvalid.Text = Strings.General_FileLocationInvalid;
                CanSave = false;
            }
            else
                if (!text.EndsWith(".txt"))
                {
                    // The file is not a text file
                    textInvalid.Text = Strings.General_FileFormatNotValid;
                    CanSave = false;
                }
                else
                    if (!File.Exists(text))
                    {
                        // The file doesn't exists
                        textInvalid.Text = Strings.General_FileNotFound;
                        CanSave = false;
                    }

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }

        private void Browse_File_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();

            dlg.FileName = FileName;
            dlg.DefaultExt = ".txt";

            if (dlg.ShowDialog().Equals(DialogResult.OK))
            {
                FileName = dlg.FileName;
                LocationBox.Text = FileName;
            }
        }

        private void Browse_Folder_Click(object sender, RoutedEventArgs e)
        {
            var dlgFolder = new FolderBrowserDialog();

            if (dlgFolder.ShowDialog().Equals(DialogResult.OK))
            {
                LocationBox.Text = dlgFolder.SelectedPath;
            }
        }

        private void Create_File_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FileStream stream = File.Create(LocationBox.Text);
                stream.Close();

                CheckValidity();
            }
            catch (Exception ex)
            {
                textInvalid.Text = Strings.PowerPoint_CantCreateFile;
                CanSave = false;
                Logger.Write(ex);
            }
        }

        private void LocationBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();
        }
    }
}
