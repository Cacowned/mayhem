using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace ConnectivityModule.Wpf
{
    /// <summary>
    /// User Control for setting the name of a bluetooth device.
    /// </summary>
    public partial class PredefinedDeviceVisibleConfig : BTBaseConfig
    {
        /// <summary>
        /// The wait time between checks.
        /// </summary>
        public int Seconds
        {
            get;
            private set;
        }

        public override string Title
        {
            get { return configTitle; }
        }

        private string configTitle;

        public PredefinedDeviceVisibleConfig(string deviceName, int seconds, string title, string deviceType, string informationText)
        {
            InitializeComponent();

            DeviceName = deviceName;
            Seconds = seconds;
            configTitle = title;
            DeviceType.Text = deviceType;
            InformationText.Text = informationText;
        }

        public override void OnLoad()
        {
            DeviceBox.Text = DeviceName;

            // The minimum timespan must be 5.
            if (Seconds < 5)
            {
                Seconds = 5;
            }

            SecondsBox.Text = Seconds.ToString(CultureInfo.InvariantCulture);

            CheckValidity();
        }

        public override void OnSave()
        {
            DeviceName = DeviceBox.Text;
            Seconds = int.Parse(SecondsBox.Text);
        }

        private void CheckValidity()
        {
            // We need to check if the network name and the number of seconds are setted correctly.
            string errorString = string.Empty;

            errorString = CheckValidityDeviceName(DeviceBox.Text);

            if (errorString.Equals(string.Empty))
            {
                errorString = CheckValiditySeconds();
            }

            DisplayErrorMessage(errorString);
        }

        private string CheckValiditySeconds()
        {
            int seconds;
            string errorString = string.Empty;

            bool badsec = !(int.TryParse(SecondsBox.Text, out seconds) && (seconds >= 5 && seconds < 60));

            if (badsec)
            {
                if (seconds < 5)
                {
                    errorString = Strings.BT_SecondsMoreThan5;
                }
                else
                {
                    if (seconds == 0)
                    {
                        errorString = Strings.WiFi_Seconds_GreaterThanZero;
                    }
                    else
                    {
                        errorString = Strings.WiFi_Seconds_Invalid;
                    }
                }
            }

            CanSave = !badsec && seconds != 0;

            return errorString;
        }

        private void DeviceBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();
        }

        private void SecondsBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
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
