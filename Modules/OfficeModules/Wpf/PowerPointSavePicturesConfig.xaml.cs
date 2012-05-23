using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using MayhemWpf.UserControls;
using OfficeModules.Resources;

namespace OfficeModules.Wpf
{
    /// <summary>
    /// Interaction logic for PowerPointSavePicturesConfig.xaml
    /// </summary>
    public partial class PowerPointSavePicturesConfig : WpfConfiguration
    {
        public PowerPointSavePicturesConfig(string filename)
        {
            InitializeComponent();

            Filename = filename;
        }

        public string Filename
        {
            get;
            private set;
        }

        public override string Title
        {
            get { return "Save Pictures"; }
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

            if (text.Length == 0)
            {
                textInvalid.Text = Strings.General_FileLocationInvalid;
                CanSave = false;
            }
            else
                if (!Directory.Exists(text))
                {
                    //the directory doesn't exists
                    textInvalid.Text = Strings.General_DirectoryNotFound;
                    CanSave = false;
                }

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }

        // Browse for folder
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
