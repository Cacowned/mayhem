using MayhemWpf.UserControls;

namespace ConnectivityModule.Wpf
{
    public class WiFiBaseConfig : WpfConfiguration
    {
        public string NetworkName
        {
            get;
            protected set;
        }

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
