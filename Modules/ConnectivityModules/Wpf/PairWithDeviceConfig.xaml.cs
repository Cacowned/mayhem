using System.Windows;
using System.Windows.Controls;

namespace ConnectivityModule.Wpf
{
    /// <summary>
    /// User Control for setting the address and pin of the bluetooth device we want to pair with.
    /// </summary>
    public partial class PairWithDeviceConfig : BTPairConfig
    {
        /// <summary>
        /// The title of the user control.
        /// </summary>
        public override string Title
        {
            get { return configTitle; }
        }

        private string configTitle;
        /// <summary>
        /// The constructor the of PairWithDeviceByAddressConfig class.
        /// </summary>
        /// <param name="deviceAddress">The address of the bluetooth device</param>
        /// <param name="pin">The pin used for pairing with the device</param>
        /// <param name="title">The title of the config window</param>
        /// <param name="deviceType">The type of the connection mode to the device</param>
        /// <param name="informationText">The information that will be displayed in the config window</param>
        public PairWithDeviceConfig(string deviceAddress, string pin, string title, string deviceType, string informationText)
        {
            InitializeComponent();

            DeviceAddress = deviceAddress;
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
            if (configTitle.Equals(Strings.PairWithDeviceByAddress_Title))
            {
                DeviceBox.Text = DeviceAddress;
            }
            else
                if (configTitle.Equals(Strings.PairWithDeviceByName_Title))
                {
                    DeviceBox.Text = DeviceName;
                }

            PinBox.Text = Pin;

            CheckValidity();
        }

        /// <summary>
        /// This method will be called when the used clicks the save button.
        /// </summary>
        public override void OnSave()
        {
            if (configTitle.Equals(Strings.PairWithDeviceByAddress_Title))
            {
                DeviceAddress = DeviceBox.Text;
            }
            else
                if (configTitle.Equals(Strings.PairWithDeviceByName_Title))
                {
                    DeviceName = DeviceBox.Text;
                }

            Pin = PinBox.Text;
        }

        /// <summary>
        /// This method will check if all the information from the user control are setted correctly.
        /// </summary>
        private void CheckValidity()
        {
            string errorString = string.Empty;
            CanSave = true;

            if (configTitle.Equals(Strings.PairWithDeviceByAddress_Title))
            {
                errorString = CheckValidityDeviceAddress(DeviceBox.Text);
            }
            else
                if (configTitle.Equals(Strings.PairWithDeviceByName_Title))
                {
                    errorString = CheckValidityDeviceName(DeviceBox.Text);
                }

            if (errorString.Equals(string.Empty))
            {
                errorString = CheckValidityPin(PinBox.Text);
            }

            DisplayErrorMessage(errorString);
        }

        /// <summary>
        /// This method will be called when the text from the DeviceAddressBox changes.
        /// </summary>
        private void DeviceBox_TextChanged(object sender, TextChangedEventArgs e)
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
