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
        public string Filename
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

            Filename = filename;
        }

        public override void OnLoad()
        {
            LocationBox.Text = Filename;
            CheckValidity();
        }

        public override void OnSave()
        {
            Filename = LocationBox.Text;
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

            dlg.FileName = Filename;
            dlg.DefaultExt = ".txt";

            if (dlg.ShowDialog().Equals(DialogResult.OK))
            {
                Filename = dlg.FileName;
                LocationBox.Text = Filename;
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
