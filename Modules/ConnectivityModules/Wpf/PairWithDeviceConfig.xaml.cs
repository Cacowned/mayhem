using System.Windows;
using System.Windows.Controls;

namespace ConnectivityModule.Wpf
{
    /// <summary>
    /// User Control for setting the address and pin of the bluetooth device we want to pair with.
    /// </summary>
    public partial class PairWithDeviceConfig : BTPairConfig
    {
        public override string Title
        {
            get { return configTitle; }
        }

        private string configTitle;

        public PairWithDeviceConfig(string deviceAddress, string pin, string title, string deviceType, string informationText)
        {
            InitializeComponent();

            DeviceAddress = deviceAddress;
            Pin = pin;
            configTitle = title;
            DeviceType.Text = deviceType;
            InformationText.Text = informationText;
        }

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

        private void DeviceBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();
        }

        private void PinBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();
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
