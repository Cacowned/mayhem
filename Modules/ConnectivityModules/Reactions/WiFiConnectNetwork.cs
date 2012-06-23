using System.Runtime.Serialization;
using MayhemCore;

namespace ConnectivityModule.Reactions
{
    [DataContract]
    [MayhemModule("WiFi: Connect To Network", "Connects to a specific network")]
    public class WiFiConnectNetwork : WiFiReactionBaseClass
    {
        public override void Perform()
        {
            string command = "netsh wlan connect name=\"" + networkName + "\"";
            string response = ChangeConnectivityMode(command);

            if (response.Equals(Strings.Succes))
            {
                ErrorLog.AddError(ErrorType.Message, string.Format(Strings.WiFi_SuccessConnectedNetwork, networkName));
            }
        }
    }
}
