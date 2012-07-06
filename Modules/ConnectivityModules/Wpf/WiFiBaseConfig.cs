using MayhemWpf.UserControls;

namespace ConnectivityModule.Wpf
{
    public class WiFiBaseConfig : WpfConfiguration
    {
        /// <summary>
        /// The name of the network.
        /// </summary>
        public string NetworkName
        {
            get;
            protected set;
        }

        /// <summary>
        /// This method will check if the name of the network is valid.
        /// </summary>
        /// <returns>An error string that will be displayed in the user control</returns>
        protected string CheckValidityNetworkName(string networkName)
        {
            int textLength = networkName.Length;
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
    }
}
