using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using MayhemWpf.UserControls;
using OfficeModules.Resources;

namespace OfficeModules.Wpf
{
    /// <summary>
    /// User Control for setting the location where the pictures of the active document will be saved.
    /// </summary>
    public partial class WordSavePicturesConfig : WpfConfiguration
    {
        /// <summary>
        /// The path of the folder where the pictures will be saved.
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
            get { return "Word: Save Pictures"; }
        }

        /// <summary>
        /// The constructor of the WordSavePicturesConfig class.
        /// </summary>
        /// <param name="filename">The path of the folder where the pictures will be saved.</param>
        public WordSavePicturesConfig(string filename)
        {
            InitializeComponent();

            FileName = filename;
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
                if (!Directory.Exists(text))
                {
                    // The directory doesn't exists
                    textInvalid.Text = Strings.General_DirectoryNotFound;
                    CanSave = false;
                }

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }

        /// <summary>
        /// This method will let the user select the location where the pictures will be saved.
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
        /// This method will be called when the text from the LocationBox changes.
        /// </summary>
        private void LocationBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();
        }
    }
}
