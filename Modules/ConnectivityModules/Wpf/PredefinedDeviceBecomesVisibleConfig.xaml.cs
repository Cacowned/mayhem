using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using MayhemWpf.UserControls;

namespace ConnectivityModule.Wpf
{
    /// <summary>
    /// User Control for setting the name of a bluetooth device.
    /// </summary>
    public partial class PredefinedDeviceBecomesVisibleConfig : WpfConfiguration
    {
        /// <summary>
        /// The name of the device.
        /// </summary>
        public string DeviceName
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
            get { return Strings.PredefinedDeviceBecomesVisible_Title; }
        }

        /// <summary>
        /// The constructor the of DeviceNameConfig class.
        /// </summary>
        /// <param name="deviceName">The name of the bluetooth device</param>
        /// <param name="seconds">The wait time between checks</param>
        public PredefinedDeviceBecomesVisibleConfig(string deviceName, int seconds)
        {
            DeviceName = deviceName;
            Seconds = seconds;

            InitializeComponent();
        }

        /// <summary>
        /// This method will be called when the user control will start loading.
        /// </summary>
        public override void OnLoad()
        {
            CanSave = true;

            DeviceNameBox.Text = DeviceName;

            // The minimum timespan must be 5.
            if (Seconds < 5)
            {
                Seconds = 5;
            }

            SecondsBox.Text = Seconds.ToString(CultureInfo.InvariantCulture);

            // We need to check if the device name and the number of seconds are setted correctly.
            string errorString = CheckValidityDeviceName();

            if (!errorString.Equals(string.Empty))
            {
                textInvalid.Text = errorString;
                textInvalid.Visibility = Visibility.Visible;
                return;
            }

            DisplayErrorMessage(CheckValiditySeconds());
        }

        /// <summary>
        /// This method will be called when the used clicks the save button.
        /// </summary>
        public override void OnSave()
        {
            DeviceName = DeviceNameBox.Text;
            Seconds = int.Parse(SecondsBox.Text);
        }

        /// <summary>
        /// This method will check if the name of the device is valid.
        /// </summary>
        /// <returns>An error string that will be displayed in the user control</returns>
        private string CheckValidityDeviceName()
        {
            int textLength = DeviceNameBox.Text.Length;
            string errorString = string.Empty;

            if (textLength == 0)
            {
                errorString = Strings.BT_DeviceName_NoCharacter;
            }
            else
            {
                if (textLength > 100)
                {
                    errorString = Strings.BT_DeviceName_TooLong;
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
        private void DeviceNameBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            DisplayErrorMessage(CheckValidityDeviceName());
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
