using System.IO;
using System.Windows;
using System.Windows.Controls;
using MayhemWpf.UserControls;
using Microsoft.Win32;

namespace ConnectivityModule.Wpf
{
    public partial class SendFileToDeviceByNameConfig : WpfConfiguration
    {
        public string DeviceName
        {
            get;
            private set;
        }

        public string FilePath
        {
            get;
            private set;
        }

        public string Pin
        {
            get;
            private set;
        }

        public override string Title
        {
            get { return Strings.SendFileToDevice_Title; }
        }

        public SendFileToDeviceByNameConfig(string deviceName, string filePath, string pin)
        {
            DeviceName = deviceName;
            FilePath = filePath;
            Pin = pin;

            InitializeComponent();
        }

        public override void OnLoad()
        {
            DeviceNameBox.Text = DeviceName;
            FilePathBox.Text = FilePath;
            PinBox.Text = Pin;

            CheckValidity();
        }

        public override void OnSave()
        {
            DeviceName = DeviceNameBox.Text;
            FilePath = FilePathBox.Text;
            Pin = PinBox.Text;
        }

        private void CheckValidity()
        {
            string errorString = string.Empty;
            CanSave = true;

            errorString = CheckValidityDeviceName();

            if (errorString.Equals(string.Empty))
            {
                errorString = CheckValidityFilePath();
                if (errorString.Equals(string.Empty))
                    errorString = CheckValidityPin();
            }

            if (!errorString.Equals(string.Empty))
            {
                textInvalid.Text = errorString;
                CanSave = false;
            }

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }

        private string CheckValidityDeviceName()
        {
            int textLength = DeviceNameBox.Text.Length;
            string errorString = string.Empty;

            if (textLength == 0)
                errorString = Strings.BT_DeviceName_NoCharacter;
            else
                if (textLength > 20)
                    errorString = Strings.BT_DeviceName_TooLong;

            CanSave = textLength != 0 && textLength <= 20;

            return errorString;
        }

        private string CheckValidityPin()
        {
            int textLength = PinBox.Text.Length;
            string errorString = string.Empty;

            if (textLength > 10)
                errorString = Strings.BT_Pin_TooLong;

            // It is ok if the pin is not setted
            CanSave = textLength == 0 || textLength <= 10;

            return errorString;
        }

        private string CheckValidityFilePath()
        {
            string text = FilePathBox.Text;
            string errorString = string.Empty;

            CanSave = true;

            do
            {
                if (text.Length == 0)
                {
                    errorString = Strings.General_FileLocationInvalid;
                    CanSave = false;
                    break;
                }

                if (!File.Exists(text))
                {
                    // The file doesn't exists
                    errorString = Strings.General_FileNotFound;
                    CanSave = false;
                    break;
                }
            } while (false);

            return errorString;
        }

        private void DeviceNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string errorString = CheckValidityDeviceName();

            // In the case that we have an error message we display it
            if (!errorString.Equals(string.Empty))
            {
                textInvalid.Text = errorString;
                textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
            }
            else
                CheckValidity(); // In the case that this is correct I need to verify the rest of the fields
        }

        private void PinBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string errorString = CheckValidityPin();

            // In the case that we have an error message we display it
            if (!errorString.Equals(string.Empty))
            {
                textInvalid.Text = errorString;
                textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
            }
            else
                CheckValidity();
        }

        private void FilePathBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string errorString = CheckValidityFilePath();

            // In the case that we have an error message we display it
            if (!errorString.Equals(string.Empty))
            {
                textInvalid.Text = errorString;
                textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
            }
            else
                CheckValidity();
        }

        private void Browse_File_Click(object sender, RoutedEventArgs e)
        {
            var dlg = new OpenFileDialog();

            dlg.FileName = FilePath;

            if (dlg.ShowDialog().Equals(true))
            {
                FilePath = dlg.FileName;
                FilePathBox.Text = FilePath;
            }
        }
    }
}
