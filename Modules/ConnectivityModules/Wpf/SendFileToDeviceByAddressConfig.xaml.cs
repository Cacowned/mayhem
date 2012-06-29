using System.IO;
using System.Windows;
using System.Windows.Controls;
using MayhemWpf.UserControls;
using Microsoft.Win32;

namespace ConnectivityModule.Wpf
{
    /// <summary>
    /// User Control for setting the address, file path and pin of the bluetooth device we want to send the file to.
    /// </summary>
    public partial class SendFileToDeviceByAddressConfig : WpfConfiguration
    {
        /// <summary>
        /// The address of the device.
        /// </summary>
        public string DeviceAddress
        {
            get;
            private set;
        }

        /// <summary>
        /// The path of the file.
        /// </summary>
        public string FilePath
        {
            get;
            private set;
        }

        /// <summary>
        /// The pin used for pairing with the device.
        /// </summary>
        public string Pin
        {
            get;
            private set;
        }

        /// <summary>
        /// The title of the user control.
        /// </summary>
        public override string Title
        {
            get { return Strings.SendFileToDevice_Title; }
        }

        /// <summary>
        /// The constructor the of SendFileToDeviceByAddressConfig class.
        /// </summary>
        /// <param name="deviceAddress">The address of the bluetooth device</param>
        /// <param name="filePath">The path of the file</param>
        /// <param name="pin">The pin used for pairing with the device</param>
        public SendFileToDeviceByAddressConfig(string deviceAddress, string filePath, string pin)
        {
            DeviceAddress = deviceAddress;
            FilePath = filePath;
            Pin = pin;

            InitializeComponent();
        }

        /// <summary>
        /// This method will be called when the user control will start loading.
        /// </summary>
        public override void OnLoad()
        {
            DeviceAddressBox.Text = DeviceAddress;
            FilePathBox.Text = FilePath;
            PinBox.Text = Pin;

            CheckValidity();
        }

        /// <summary>
        /// This method will be called when the used clicks the save button.
        /// </summary>
        public override void OnSave()
        {
            DeviceAddress = DeviceAddressBox.Text;
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

            errorString = CheckValidityDeviceAddress();

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

        /// <summary>
        /// This method will check if the address of the device is valid.
        /// </summary>
        /// <returns>An error string that will be displayed in the user control</returns>
        private string CheckValidityDeviceAddress()
        {
            int textLength = DeviceAddressBox.Text.Length;
            string errorString = string.Empty;

            if (textLength < 12)
                errorString = Strings.BT_DeviceAddress_TooShort;
            else
                if (textLength > 17)
                    errorString = Strings.BT_DeviceAddress_TooLong;

            CanSave = textLength >= 12 && textLength <= 17;

            return errorString;
        }

        /// <summary>
        /// This method will check if the pin is valid.
        /// </summary>
        /// <returns>An error string that will be displayed in the user control</returns>
        private string CheckValidityPin()
        {
            int textLength = PinBox.Text.Length;
            string errorString = string.Empty;

            if (textLength > 10)
                errorString = Strings.BT_Pin_TooLong;

            // It is ok if the pin is not setted.
            CanSave = textLength == 0 || textLength <= 10;

            return errorString;
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
            string errorString = CheckValidityDeviceAddress();

            // In the case that we have an error message we display it.
            if (!errorString.Equals(string.Empty))
            {
                textInvalid.Text = errorString;
                textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
            }
            else
                CheckValidity(); // In the case that this is correct we need to verify the rest of the fields.
        }

        /// <summary>
        /// This method will be called when the text from the PinBox changes.
        /// </summary>
        private void PinBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string errorString = CheckValidityPin();

            // In the case that we have an error message we display it.
            if (!errorString.Equals(string.Empty))
            {
                textInvalid.Text = errorString;
                textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
            }
            else
                CheckValidity();
        }

        /// <summary>
        /// This method will be called when the text from the FilePathBox changes.
        /// </summary>
        private void FilePathBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string errorString = CheckValidityFilePath();

            // In the case that we have an error message we display it.
            if (!errorString.Equals(string.Empty))
            {
                textInvalid.Text = errorString;
                textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
            }
            else
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
    }
}
