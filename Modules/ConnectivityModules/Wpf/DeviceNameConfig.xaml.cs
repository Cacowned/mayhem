using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using MayhemWpf.UserControls;

namespace ConnectivityModule.Wpf
{
    public partial class DeviceNameConfig : WpfConfiguration
    {
        public string DeviceName
        {
            get;
            private set;
        }

        public int Seconds
        {
            get;
            private set;
        }

        public override string Title
        {
            get { return Strings.DeviceName_Title; }
        }

        public DeviceNameConfig(string deviceName, int seconds)
        {
            DeviceName = deviceName;
            Seconds = seconds;

            InitializeComponent();
        }

        public override void OnLoad()
        {
            CanSave = true;

            DeviceNameBox.Text = DeviceName;

            // The minimum time span must be 5
            if (Seconds < 5)
                Seconds = 5;

            SecondsBox.Text = Seconds.ToString(CultureInfo.InvariantCulture);

            // We need to check if the device name and the number of seconds are setted correctly
            string errorString = CheckValidityDeviceName();

            if (!errorString.Equals(string.Empty))
            {
                textInvalid.Text = errorString;
                textInvalid.Visibility = Visibility.Visible;
                return;
            }

            errorString = CheckValiditySeconds();

            if (!errorString.Equals(string.Empty))
                textInvalid.Text = errorString;

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }

        public override void OnSave()
        {
            DeviceName = DeviceNameBox.Text;
            Seconds = int.Parse(SecondsBox.Text);
        }

        private string CheckValidityDeviceName()
        {
            int textLength = DeviceNameBox.Text.Length;
            string errorString = string.Empty;

            if (textLength == 0)
                errorString = Strings.BT_DeviceName_NoCharacter;
            else
                if (textLength > 100)
                    errorString = Strings.BT_DeviceName_TooLong;

            CanSave = textLength > 0 && (textLength <= 100);

            return errorString;
        }

        private string CheckValiditySeconds()
        {
            int seconds;
            string errorString = string.Empty;

            bool badsec = !(int.TryParse(SecondsBox.Text, out seconds) && (seconds >= 5 && seconds < 60));

            if (badsec)
                if (seconds < 5)
                    errorString = Strings.BT_SecondsMoreThan5;
                else
                    if (seconds == 0)
                        errorString = Strings.WiFi_Seconds_GreaterThanZero;
                    else
                        errorString = Strings.WiFi_Seconds_Invalid;

            CanSave = !badsec && seconds != 0;

            return errorString;
        }

        private void DeviceNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string errorString = CheckValidityDeviceName();

            // In the case that we have an error message we display it
            if (!errorString.Equals(string.Empty))
                textInvalid.Text = errorString;

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }

        private void SecondsBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            string errorString = CheckValiditySeconds();

            // In the case that we have an error message we display it
            if (!errorString.Equals(string.Empty))
                textInvalid.Text = errorString;

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
