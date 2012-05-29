using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using MayhemWpf.UserControls;
using OfficeModules.Resources;

namespace OfficeModules.Wpf
{
    public partial class WordSavePicturesConfig : WpfConfiguration
    {
        public string Filename
        {
            get;
            private set;
        }

        public override string Title
        {
            get { return "Save Pictures"; }
        }

        public WordSavePicturesConfig(string filename)
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
