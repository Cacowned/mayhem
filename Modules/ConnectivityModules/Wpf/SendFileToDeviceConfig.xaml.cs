using System.IO;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Win32;

namespace ConnectivityModule.Wpf
{
    /// <summary>
    /// User Control for setting the address, file path and pin of the bluetooth device we want to send the file to.
    /// </summary>
    public partial class SendFileToDeviceConfig : BTPairConfig
    {
        /// <summary>
        /// The path of the file that will be sent to the selected bluetooth device.
        /// </summary>
        public string FilePath
        {
            get;
            private set;
        }

        public override string Title
        {
            get { return configTitle; }
        }

        private string configTitle;

        public SendFileToDeviceConfig(string deviceAddress, string filePath, string pin, string title, string deviceType, string informationText)
        {
            InitializeComponent();

            DeviceAddress = deviceAddress;
            FilePath = filePath;
            Pin = pin;
            configTitle = title;
            DeviceType.Text = deviceType;
            InformationText.Text = informationText;
        }

        public override void OnLoad()
        {
            if (configTitle.Equals(Strings.SendFileToDeviceByAddress_Title))
            {
                DeviceBox.Text = DeviceAddress;
            }
            else
                if (configTitle.Equals(Strings.SendFileToDeviceByName_Title))
                {
                    DeviceBox.Text = DeviceName;
                }
            FilePathBox.Text = FilePath;
            PinBox.Text = Pin;

            CheckValidity();
        }

        public override void OnSave()
        {
            if (configTitle.Equals(Strings.SendFileToDeviceByAddress_Title))
            {
                DeviceAddress = DeviceBox.Text;
            }
            else
                if (configTitle.Equals(Strings.SendFileToDeviceByName_Title))
                {
                    DeviceName = DeviceBox.Text;
                }

            FilePath = FilePathBox.Text;
            Pin = PinBox.Text;
        }

        private void CheckValidity()
        {
            string errorString = string.Empty;
            CanSave = true;

            if (configTitle.Equals(Strings.SendFileToDeviceByAddress_Title))
            {
                errorString = CheckValidityDeviceAddress(DeviceBox.Text);
            }
            else
                if (configTitle.Equals(Strings.SendFileToDeviceByName_Title))
                {
                    errorString = CheckValidityDeviceName(DeviceBox.Text);
                }

            if (errorString.Equals(string.Empty))
            {
                errorString = CheckValidityFilePath();
            }

            if (errorString.Equals(string.Empty))
            {
                errorString = CheckValidityPin(PinBox.Text);
            }

            DisplayErrorMessage(errorString);
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
                    // The file doesn't exists.
                    errorString = Strings.General_FileNotFound;
                    CanSave = false;
                    break;
                }
            } while (false);

            return errorString;
        }

        private void DeviceAddressBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();
        }

        private void PinBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();
        }

        private void FilePathBox_TextChanged(object sender, TextChangedEventArgs e)
        {
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

        private void DisplayErrorMessage(string errorString)
        {
            if (!errorString.Equals(string.Empty))
            {
                textInvalid.Text = errorString;
            }

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
