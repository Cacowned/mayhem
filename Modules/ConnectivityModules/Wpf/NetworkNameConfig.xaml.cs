using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using MayhemWpf.UserControls;

namespace ConnectivityModule.Wpf
{
    /// <summary>
    /// User Control for setting the name of a network.
    /// </summary>
    public partial class NetworkNameConfig : WpfConfiguration
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
        /// The wait time between checks.
        /// </summary>
        public int Seconds
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
        /// The constructor of the NetworkNameConfig class.
        /// </summary>
        /// <param name="networkName">The name of the network</param>
        public NetworkNameConfig(string networkName, int seconds)
        {
            NetworkName = networkName;
            Seconds = seconds;

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

            // We need to check if the network name and the number of seconds are setted correctly.
            string errorString = CheckValidityNetworkName();

            if (!errorString.Equals(string.Empty))
            {
                textInvalid.Text = errorString;
                textInvalid.Visibility = Visibility.Visible;
                return;
            }

            errorString = CheckValiditySeconds();

            if (!errorString.Equals(string.Empty))
            {
                textInvalid.Text = errorString;
            }

            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
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
        /// This method will check if the name of the network is valid.
        /// </summary>
        /// <returns>An error string that will be displayed in the user control</returns>
        private string CheckValidityNetworkName()
        {
            int textLength = NetworkNameBox.Text.Length;
            string errorString = string.Empty;

            if (textLength == 0)
            {
                errorString = Strings.WiFi_NetworkName_NoCharacter;
            }
            else
            {
                if (textLength > 100)
                {
                    errorString = Strings.WiFi_NetworkName_TooLong;
                }
            }

            CanSave = textLength > 0 && (textLength <= 100);

            return errorString;
        }

        /// <summary>
        /// This method will check if the number of seconds is setted correctly.
        /// </summary>
        /// <returns>An error string that will be displayed in the user control</returns>
        private string CheckValiditySeconds()
        {
            int seconds;
            string errorString = string.Empty;

            bool badsec = !(int.TryParse(SecondsBox.Text, out seconds) && (seconds >= 0 && seconds < 60));

            if (badsec)
            {
                errorString = Strings.WiFi_Seconds_Invalid;
            }
            else
            {
                if (seconds == 0)
                {
                    errorString = Strings.WiFi_Seconds_GreaterThanZero;
                }
            }

            CanSave = !badsec && seconds != 0;

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
        /// This method will be called when the text from the DeviceNameBox changes.
        /// </summary>
        private void SecondsBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            DisplayErrorMessage(CheckValiditySeconds());
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
