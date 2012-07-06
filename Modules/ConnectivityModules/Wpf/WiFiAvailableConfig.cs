namespace ConnectivityModule.Wpf
{
    public class WiFiAvailableConfig : WiFiBaseConfig
    {
        /// <summary>
        /// The wait time between checks.
        /// </summary>
        public int Seconds
        {
            get;
            protected set;
        }

        /// <summary>
        /// This method will check if the number of seconds is setted correctly.
        /// </summary>
        /// <returns>An error string that will be displayed in the user control</returns>
        protected string CheckValiditySeconds(string secondsText)
        {
            int seconds;
            string errorString = string.Empty;

            bool badsec = !(int.TryParse(secondsText, out seconds) && (seconds >= 0 && seconds < 60));

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
    }
}
