using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using MayhemWpf.UserControls;
using OfficeModules.Resources;

namespace OfficeModules.Wpf
{
    /// <summary>
    /// User Control for setting the location where the pictures of the current slide will be saved.
    /// </summary>
    public partial class PowerPointSavePicturesConfig : WpfConfiguration
    {
        /// <summary>
        /// The path of the folder where the pictures will be saved.
        /// </summary>
        public string FileName
        {
            get;
            private set;
        }

        public override string Title
        {
            get { return "PowerPoint: Save Pictures"; }
        }

        public PowerPointSavePicturesConfig(string filename)
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
                if (!Directory.Exists(text))
                {
                    // The directory doesn't exists
                    textInvalid.Text = Strings.General_DirectoryNotFound;
                    CanSave = false;
                }

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }

        private void Browse_Folder_Click(object sender, RoutedEventArgs e)
        {
            var dlgFolder = new FolderBrowserDialog();

            if (dlgFolder.ShowDialog().Equals(DialogResult.OK))
            {
                LocationBox.Text = dlgFolder.SelectedPath;
            }
        }

        private void LocationBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();
        }
    }
}
