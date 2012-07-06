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
        /// The path of the file.
        /// </summary>
        public string FilePath
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
        /// The constructor the of SendFileToDeviceByAddressConfig class.
        /// </summary>
        /// <param name="deviceAddress">The address of the bluetooth device</param>
        /// <param name="filePath">The path of the file</param>
        /// <param name="pin">The pin used for pairing with the device</param>
        /// <param name="title">The title of the config window</param>
        /// <param name="deviceType">The type of the connection mode to the device</param>
        /// <param name="informationText">The information text that will be displayed in the config window</param>
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

        /// <summary>
        /// This method will be called when the user control will start loading.
        /// </summary>
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

        /// <summary>
        /// This method will be called when the used clicks the save button.
        /// </summary>
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

        /// <summary>
        /// This method will check if all the information from the user control are setted correctly.
        /// </summary>
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

        /// <summary>
        /// This method will check if the path of the file is valid.
        /// </summary>
        /// <returns>An error string that will be displayed in the user control</returns>
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

        /// <summary>
        /// This method will be called when the text from the DeviceAddressBox changes.
        /// </summary>
        private void DeviceAddressBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();
        }

        /// <summary>
        /// This method will be called when the text from the PinBox changes.
        /// </summary>
        private void PinBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();
        }

        /// <summary>
        /// This method will be called when the text from the FilePathBox changes.
        /// </summary>
        private void FilePathBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();
        }

        /// <summary>
        /// This method will open an OpenFileDialog control and will allow the user the select the file he wants to send to the bluetooth device.
        /// </summary>
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

        /// <summary>
        /// Displays the error message received as parameter.
        /// </summary>
        /// <param name="errorMessage">The text of the error message</param>
        private void DisplayErrorMessage(string errorString)
        {
            // In the case that we have an error message we display it.
            if (!errorString.Equals(string.Empty))
            {
                textInvalid.Text = errorString;
            }

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
