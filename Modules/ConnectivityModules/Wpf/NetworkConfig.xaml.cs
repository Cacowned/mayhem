using System.Windows;
using System.Windows.Controls;

namespace ConnectivityModule.Wpf
{
    /// <summary>
    /// User Control for setting the name of the network we want to connect to, or disconnect from.
    /// </summary>
    public partial class NetworkConfig : WiFiBaseConfig
    {
        public override string Title
        {
            get { return configTitle; }
        }

        private string configTitle;

        public NetworkConfig(string networkName, string title, string informationText)
        {
            InitializeComponent();

            NetworkName = networkName;
            configTitle = title;
            InformationText.Text = informationText;
        }

        public override void OnLoad()
        {
            CanSave = true;

            NetworkNameBox.Text = NetworkName;

            DisplayErrorMessage(CheckValidityNetworkName(NetworkNameBox.Text));
        }

        public override void OnSave()
        {
            NetworkName = NetworkNameBox.Text;
        }

        private void NetworkNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DisplayErrorMessage(CheckValidityNetworkName(NetworkNameBox.Text));
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
