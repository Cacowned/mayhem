using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace ConnectivityModule.Wpf
{
    /// <summary>
    /// User Control for setting the name of a network.
    /// </summary>
    public partial class NetworkAvailableConfig : WiFiAvailableConfig
    {
        public override string Title
        {
            get { return configTitle; }
        }

        private string configTitle;

        public NetworkAvailableConfig(string networkName, int seconds, string title)
        {
            NetworkName = networkName;
            Seconds = seconds;
            configTitle = title;

            InitializeComponent();
        }

        public override void OnLoad()
        {
            CanSave = true;

            NetworkNameBox.Text = NetworkName;

            // The minimum time span must be 1.
            if (Seconds == 0)
            {
                Seconds = 1;
            }

            SecondsBox.Text = Seconds.ToString(CultureInfo.InvariantCulture);

            CheckValidity();
        }

        public override void OnSave()
        {
            NetworkName = NetworkNameBox.Text;
            Seconds = int.Parse(SecondsBox.Text);
        }

        private void CheckValidity()
        {
            // We need to check if the network name and the number of seconds are setted correctly.
            string errorString = CheckValidityNetworkName(NetworkNameBox.Text);

            if (errorString.Equals(string.Empty))
            {
                errorString = CheckValiditySeconds(SecondsBox.Text);
            }

            DisplayErrorMessage(errorString);
        }

        private void NetworkNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();
        }

        private void SecondsBox_TextChanged(object sender, TextChangedEventArgs e)
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
