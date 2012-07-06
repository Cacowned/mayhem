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

        /// <summary>
        /// The title of the user control.
        /// </summary>
        public override string Title
        {
            get { return configTitle; }
        }

        private string configTitle;

        /// <summary>
        /// The constructor the of DeviceNameConfig class.
        /// </summary>
        /// <param name="deviceName">The name of the bluetooth device</param>
        /// <param name="seconds">The wait time between checks</param
        /// <param name="title">The title of the config window</param>
        /// <param name="deviceType">The type of the connection mode to the device</param>
        /// <param name="informationText">The information that will be displayed in the config window</param>>
        public PredefinedDeviceVisibleConfig(string deviceName, int seconds, string title, string deviceType, string informationText)
        {
            InitializeComponent();

            DeviceName = deviceName;
            Seconds = seconds;
            configTitle = title;
            DeviceType.Text = deviceType;
            InformationText.Text = informationText;
        }

        /// <summary>
        /// This method will be called when the user control will start loading.
        /// </summary>
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

        /// <summary>
        /// This method will be called when the used clicks the save button.
        /// </summary>
        public override void OnSave()
        {
            DeviceName = DeviceBox.Text;
            Seconds = int.Parse(SecondsBox.Text);
        }

        /// <summary>
        /// This method will check if all the information from the user control are setted correctly.
        /// </summary>
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

        /// <summary>
        /// This method will check if the number of seconds is setted correctly.
        /// </summary>
        /// <returns>An error string that will be displayed in the user control</returns>
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

        /// <summary>
        /// This method will be called when the text from the DeviceNameBox changes.
        /// </summary>
        private void DeviceBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();
        }

        /// <summary>
        /// This method will be called when the text from the DeviceNameBox changes.
        /// </summary>
        private void SecondsBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
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
