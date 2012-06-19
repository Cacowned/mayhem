using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using MayhemWpf.UserControls;

namespace ConnectivityModule.Wpf
{
    public partial class NetworkNameConfig : WpfConfiguration
    {
        public string NetworkName
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
            get { return Strings.NetworkName_Title; }
        }

        public NetworkNameConfig(string networkName, int seconds)
        {
            NetworkName = networkName;
            Seconds = seconds;

            InitializeComponent();
        }

        public override void OnLoad()
        {
            CanSave = true;

            NetworkNameBox.Text = NetworkName;

            // The minimum time span must be 1
            if (Seconds == 0)
                Seconds = 1;

            SecondsBox.Text = Seconds.ToString(CultureInfo.InvariantCulture);

            // We need to check if the network name and the number of seconds are setted correctly
            string errorString = CheckValidityNetworkName();

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
            NetworkName = NetworkNameBox.Text;
            Seconds = int.Parse(SecondsBox.Text);
        }

        private string CheckValidityNetworkName()
        {
            int textLength = NetworkNameBox.Text.Length;
            string errorString = string.Empty;

            if (textLength == 0)
                errorString = Strings.WiFi_NetworkName_NoCharacter;
            else
                if (textLength > 100)
                    errorString = Strings.WiFi_NetworkName_TooLong;

            CanSave = textLength > 0 && (textLength <= 100);

            return errorString;
        }

        private string CheckValiditySeconds()
        {
            int seconds;
            string errorString = string.Empty;

            bool badsec = !(int.TryParse(SecondsBox.Text, out seconds) && (seconds >= 0 && seconds < 60));

            if (badsec)
                errorString = Strings.WiFi_Seconds_Invalid;
            else
                if (seconds == 0)
                    errorString = Strings.WiFi_Seconds_GreaterThanZero;

            CanSave = !badsec && seconds != 0;

            return errorString;
        }

        private void NetworkNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string errorString = CheckValidityNetworkName();

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
