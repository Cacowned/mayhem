using System.Windows;
using System.Windows.Controls;

namespace ConnectivityModule.Wpf
{
    /// <summary>
    /// User Control for setting the name of the network we want to connect to, or disconnect from.
    /// </summary>
    public partial class NetworkConfig : WiFiBaseConfig
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
        /// The constructor of the ConnectNetworkConfig class.
        /// </summary>
        /// <param name="networkName">The name of the network</param>
        public NetworkConfig(string networkName, string title, string informationText)
        {
            InitializeComponent();

            NetworkName = networkName;
            configTitle = title;
            InformationText.Text = informationText;
        }

        /// <summary>
        /// This method will be called when the user control will start loading.
        /// </summary>
        public override void OnLoad()
        {
            CanSave = true;

            NetworkNameBox.Text = NetworkName;

            DisplayErrorMessage(CheckValidityNetworkName(NetworkNameBox.Text));
        }

        /// <summary>
        /// This method will be called when the used clicks the save button.
        /// </summary>
        public override void OnSave()
        {
            NetworkName = NetworkNameBox.Text;
        }

        /// <summary>
        /// This method will be called when the text from the NetworkNameBox changes.
        /// </summary>
        private void NetworkNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DisplayErrorMessage(CheckValidityNetworkName(NetworkNameBox.Text));
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
