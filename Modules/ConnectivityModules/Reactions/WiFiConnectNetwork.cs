using System.Runtime.Serialization;
using MayhemCore;
using NativeWifi;
using System.ComponentModel;
using System;

namespace ConnectivityModule.Reactions
{
    [DataContract]
    [MayhemModule("WiFi: Connect To Network", "Connects to a specific network")]
    public class WiFiConnectNetwork : WiFiReactionBaseClass
    {
        public override void Perform()
        {
            string command = "netsh wlan connect name=\"" + networkName + "\"";

            try
            {
                bool response = ChangeConnectivityMode(command);

                if (response == false)
                {
                    return;
                }

                bool found = false;
                foreach (WlanClient.WlanInterface wlanInterface in client.Interfaces)
                {
                    try
                    {
                        Wlan.WlanConnectionAttributes connAtributes = wlanInterface.CurrentConnection;

                        if (connAtributes.profileName.Equals(networkName) && connAtributes.isState == Wlan.WlanInterfaceState.Connected)
                            found = true;
                    }
                    catch (Win32Exception ex)
                    {
                        ErrorLog.AddError(ErrorType.Failure, Strings.WiFi_CantConnectToNetwork);
                        Logger.Write(ex);

                        return;
                    }
                }

                if (found == false)
                {
                    ErrorLog.AddError(ErrorType.Failure, Strings.WiFi_CantConnectToNetwork);
                }
                else
                {
                    ErrorLog.AddError(ErrorType.Message, string.Format(Strings.WiFi_SuccessConnectedNetwork, networkName));
                }
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.WiFi_CantConnectToNetwork);
                Logger.Write(ex);
            }
        }
    }
}
