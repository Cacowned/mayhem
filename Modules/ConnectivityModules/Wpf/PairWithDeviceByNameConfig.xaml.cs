using System.Windows;
using System.Windows.Controls;
using MayhemWpf.UserControls;

namespace ConnectivityModule.Wpf
{
    /// <summary>
    /// User Control for setting the name and pin of the bluetooth device we want to pair with.
    /// </summary>
    public partial class PairWithDeviceByNameConfig : WpfConfiguration
    {
        /// <summary>
        /// The name of the device
        /// </summary>
        public string DeviceName
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
            get { return Strings.PairWithDeviceByName_Title; }
        }

        /// <summary>
        /// The constructor the of PairWithDeviceByNameConfig class.
        /// </summary>
        /// <param name="deviceName">The name of the bluetooth device</param>
        /// <param name="pin">The pin used for pairing with the device</param>
        public PairWithDeviceByNameConfig(string deviceName, string pin)
        {
            DeviceName = deviceName;
            Pin = pin;

            InitializeComponent();
        }

        /// <summary>
        /// This method will be called when the user control will start loading.
        /// </summary>
        public override void OnLoad()
        {
            DeviceNameBox.Text = DeviceName;
            PinBox.Text = Pin;

            CheckValidity();
        }

        /// <summary>
        /// This method will be called when the used clicks the save button.
        /// </summary>
        public override void OnSave()
        {
            DeviceName = DeviceNameBox.Text;
            Pin = PinBox.Text;
        }

        /// <summary>
        /// This method will check if all the information from the user control are setted correctly.
        /// </summary>
        private void CheckValidity()
        {
            string errorString = string.Empty;
            CanSave = true;

            errorString = CheckValidityDeviceName();

            if (errorString.Equals(string.Empty))
            {
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
        /// This method will check if the name of the device is valid.
        /// </summary>
        /// <returns>An error string that will be displayed in the user control</returns>
        private string CheckValidityDeviceName()
        {
            int textLength = DeviceNameBox.Text.Length;
            string errorString = string.Empty;

            if (textLength == 0)
            {
                errorString = Strings.BT_DeviceName_NoCharacter;
            }
            else
            {
                if (textLength > 20)
                {
                    errorString = Strings.BT_DeviceName_TooLong;
                }
            }

            CanSave = textLength != 0 && textLength <= 20;

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

            if (textLength == 0)
            {
                errorString = Strings.BT_Pin_NoCharacter;
            }
            else
                if (textLength > 10)
                {
                    errorString = Strings.BT_Pin_TooLong;
                }

            CanSave = textLength != 0 && textLength <= 10;

            return errorString;
        }

        /// <summary>
        /// This method will be called when the text from the DeviceNameBox changes.
        /// </summary>
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
            {
                CheckValidity(); // In the case that this is correct we need to verify the rest of the fields
            }
        }

        /// <summary>
        /// This method will be called when the text from the PinBox changes.
        /// </summary>
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
            {
                CheckValidity();
            }
        }
    }
}
