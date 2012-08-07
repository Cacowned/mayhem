using System.IO;
using System.Windows;
using Microsoft.Win32;
using SkypeModules.Resources;

namespace SkypeModules.Wpf
{
    /// <summary>
    /// User Control for setting the path of the audio file that will be sent to the user with the selected Skype ID
    /// </summary>
    public partial class SkypeAudioMessageConfig : SkypeBaseConfig
    {
        public string FilePath
        {
            get;
            private set;
        }

        public SkypeAudioMessageConfig(string skypeID, string filePath, string title)
        {
            SkypeID = skypeID;
            FilePath = filePath;
            configTitle = title;

            InitializeComponent();
        }

        public override void OnLoad()
        {
            SkypeIDBox.Text = SkypeID;
            LocationBox.Text = FilePath;

            CheckValidity();
        }

        public override void OnSave()
        {
            SkypeID = SkypeIDBox.Text;
            FilePath = LocationBox.Text;
        }

        private bool CheckValidityLocation()
        {
            errorString = string.Empty;

            if (!CheckValidityField(LocationBox.Text, Strings.FilePath, maxLength: 100))
            {
                return false;
            }
            else
                if (!LocationBox.Text.EndsWith(".wav"))
                {
                    errorString = Strings.General_FileFormatNotValid;
                    return false;
                }
                else
                    if (!File.Exists(LocationBox.Text))
                    {
                        errorString = Strings.General_FileNotFound;
                        return false;
                    }

            return true;
        }

        protected override void CheckValidity()
        {
            errorString = string.Empty;

            // The evaluation variable is not used but it won't compile if I don't store the result.
            bool evaluation = CheckValidityField(SkypeIDBox.Text, Strings.SkypeID, maxLength: 100) &&
                              CheckValidityLocation();

            DisplayErrorMessage(textInvalid);
        }

        private void Browse_File_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();

            dlg.FileName = FilePath;
            dlg.DefaultExt = ".wav";

            if (dlg.ShowDialog().Equals(true))
            {
                FilePath = dlg.FileName;
                LocationBox.Text = FilePath;
            }
        }
    }
}
