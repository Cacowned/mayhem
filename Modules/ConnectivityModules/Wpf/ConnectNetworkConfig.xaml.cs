using System.Windows;
using System.Windows.Controls;
using MayhemWpf.UserControls;

namespace ConnectivityModule.Wpf
{
    /// <summary>
    /// User Control for setting the name of the network we want to connect to, or disconnect from.
    /// </summary>
    public partial class ConnectNetworkConfig : WpfConfiguration
    {
        /// <summary>
        /// The name of the network.
        /// </summary>
        public string NetworkName
        {
            get;
            private set;
        }

        /// <summary>
        /// The title of the user control.
        /// </summary>
        public override string Title
        {
            get { return Strings.NetworkName_Title; }
        }

        /// <summary>
        /// The constructor of the ConnectNetworkConfig class.
        /// </summary>
        /// <param name="networkName">The name of the network</param>
        public ConnectNetworkConfig(string networkName)
        {
            NetworkName = networkName;

            InitializeComponent();
        }

        /// <summary>
        /// This method will be called when the user control will start loading.
        /// </summary>
        public override void OnLoad()
        {
            CanSave = true;

            NetworkNameBox.Text = NetworkName;

            DisplayErrorMessage(CheckValidityNetworkName());
        }

        /// <summary>
        /// This method will be called when the used clicks the save button.
        /// </summary>
        public override void OnSave()
        {
            NetworkName = NetworkNameBox.Text;
        }

        /// <summary>
        /// This method will check if the name of the network is valid.
        /// </summary>
        /// <returns>An error string that will be displayed in the user control</returns>
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

        /// <summary>
        /// This method will be called when the text from the NetworkNameBox changes.
        /// </summary>
        private void NetworkNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DisplayErrorMessage(CheckValidityNetworkName());
        }

        /// <summary>
        /// Displays the error message received as parameter.
        /// </summary>
        /// <param name="errorMessage">The text of the error message</param>
        private void DisplayErrorMessage(string errorString)
        {
            // In the case that we have an error message we display it.
            if (!errorString.Equals(string.Empty))
                textInvalid.Text = errorString;

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
