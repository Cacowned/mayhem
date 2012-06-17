using System.Windows;
using System.Windows.Controls;
using MayhemWpf.UserControls;

namespace ConnectivityModule.Wpf
{
    public partial class PairWithDeviceByAddressConfig : WpfConfiguration
    {
        public string DeviceAddress
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
            get { return Strings.PairWithDevice_Title; }
        }

        public PairWithDeviceByAddressConfig(string deviceAddress, string pin)
        {
            DeviceAddress = deviceAddress;
            Pin = pin;

            InitializeComponent();
        }

        public override void OnLoad()
        {
            DeviceAddressBox.Text = DeviceAddress;
            PinBox.Text = Pin;

            CheckValidity();
        }

        public override void OnSave()
        {
            DeviceAddress = DeviceAddressBox.Text;
            Pin = PinBox.Text;
        }

        private void CheckValidity()
        {
            string errorString = string.Empty;
            CanSave = true;

            errorString = CheckValidityDeviceAddress();

            if (errorString.Equals(string.Empty))
                errorString = CheckValidityPin();

            if (!errorString.Equals(string.Empty))
            {
                textInvalid.Text = errorString;
                CanSave = false;
            }

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }

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

        private void DeviceAddressBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string errorString = CheckValidityDeviceAddress();

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
    }
}
