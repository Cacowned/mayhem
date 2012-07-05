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
    /// <summary>
    /// The User Control used for setting the file for the exported information.
    /// </summary>
    public partial class PowerPointExportConfig : WpfConfiguration
    {
        /// <summary>
        /// The location of the file where the information will be saved.
        /// </summary>
        public string FileName
        {
            get;
            private set;
        }

        /// <summary>
        /// The title of the user control.
        /// </summary>
        public override string Title
        {
            get { return configTitle; }
        }

        private string configTitle;

        /// <summary>
        /// The constructor of the PowerPointExportConfig class.
        /// </summary>
        /// <param name="filename">The location of the file where the information will be saved.</param>
        /// <param name="title">The title of the config window</param>
        public PowerPointExportConfig(string filename, string title)
        {
            InitializeComponent();

            FileName = filename;
            configTitle = title;
        }

        /// <summary>
        /// This method will be called when the user control will start loading.
        /// </summary>
        public override void OnLoad()
        {
            LocationBox.Text = FileName;
            CheckValidity();
        }

        /// <summary>
        /// This method will be called when the user clicks the save button.
        /// </summary>
        public override void OnSave()
        {
            FileName = LocationBox.Text;
        }

        /// <summary>
        /// This method will check the validity of the information provided by the user.
        /// </summary>
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

        /// <summary>
        /// This method will let the user select the file where the information will be saved
        /// </summary>
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

        /// <summary>
        /// This method will let the user select the location where the information will be saved.
        /// </summary>
        private void Browse_Folder_Click(object sender, RoutedEventArgs e)
        {
            var dlgFolder = new FolderBrowserDialog();

            if (dlgFolder.ShowDialog().Equals(DialogResult.OK))
            {
                LocationBox.Text = dlgFolder.SelectedPath;
            }
        }

        /// <summary>
        /// This method will create a new file.
        /// </summary>
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

        /// <summary>
        /// This method will be called when the text from the  LocationBox changes.
        /// </summary>
        private void LocationBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();
        }
    }
}
