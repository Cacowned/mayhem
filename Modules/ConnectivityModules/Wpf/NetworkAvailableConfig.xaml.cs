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
        /// <summary>
        /// The title of the user control.
        /// </summary>
        public override string Title
        {
            get { return configTitle; }
        }

        private string configTitle;

        /// <summary>
        /// The constructor of the NetworkNameConfig class.
        /// </summary>
        /// <param name="networkName">The name of the network</param>
        /// <param name="title">The title of the config window</param>
        public NetworkAvailableConfig(string networkName, int seconds, string title)
        {
            NetworkName = networkName;
            Seconds = seconds;
            configTitle = title;

            InitializeComponent();
        }

        /// <summary>
        /// This method will be called when the user control will start loading.
        /// </summary>
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

        /// <summary>
        /// This method will be called when the used clicks the save button.
        /// </summary>
        public override void OnSave()
        {
            NetworkName = NetworkNameBox.Text;
            Seconds = int.Parse(SecondsBox.Text);
        }

        /// <summary>
        /// This method will check if all the information from the user control are setted correctly.
        /// </summary>
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

        /// <summary>
        /// This method will be called when the text from the NetworkNameBox changes.
        /// </summary>
        private void NetworkNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();
        }

        /// <summary>
        /// This method will be called when the text from the DeviceNameBox changes.
        /// </summary>
        private void SecondsBox_TextChanged(object sender, TextChangedEventArgs e)
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
