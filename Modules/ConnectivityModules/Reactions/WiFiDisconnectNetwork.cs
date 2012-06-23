using System.Runtime.Serialization;
using MayhemCore;

namespace ConnectivityModule.Reactions
{
    [DataContract]
    [MayhemModule("WiFi: Disconnect From Network", "Disconnects from a specific network")]
    public class WiFiDisconnectNetwork : WiFiReactionBaseClass
    {
        public override void Perform()
        {
            string command = "netsh wlan disconnect";
            string response = ChangeConnectivityMode(command);

            if (response.Equals(Strings.Succes))
            {
                ErrorLog.AddError(ErrorType.Message, string.Format(Strings.WiFi_SuccessConnectedNetwork, networkName));
            }
        }
    }
}
