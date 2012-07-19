using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using ConnectivityModule.Wpf;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using NativeWifi;

namespace ConnectivityModule.Reactions
{
    /// <summary>
    /// A class that disconnects from a predefined network.
    /// </summary>
    [DataContract]
    [MayhemModule("Wi-Fi: Disconnect From Network", "Disconnects from a specific network")]
    public class WiFiDisconnectNetwork : WiFiReactionBaseClass, IWpfConfigurable
    {
        protected bool VerifyConnectionState()
        {
            try
            {
                foreach (WlanClient.WlanInterface wlanInterface in client.Interfaces)
                {
                    try
                    {
                        Wlan.WlanConnectionAttributes connAtributes = wlanInterface.CurrentConnection;

                        if (connAtributes.profileName.Equals(networkName))
                        {
                            return true;
                        }
                    }
                    catch (Win32Exception ex)
                    {
                        Logger.Write(ex);

                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Write(ex);

                return false;
            }

            return false;
        }

        /// <summary>
        /// This method will try to disconnect from the predefined network.
        /// </summary>
        public override void Perform()
        {
            string command = "netsh wlan disconnect";

            try
            {
                client = new WlanClient();
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.WiFi_WiFiNotAvailable);
                Logger.Write(ex);

                return;
            }

            try
            {
                if (!VerifyConnectionState())
                {
                    ErrorLog.AddError(ErrorType.Failure, string.Format(Strings.WiFi_NoNetworkConnected, networkName));
                    return;
                }

                bool response = ChangeConnectivityMode(command);

                if (response == false)
                {
                    return;
                }

                if (!VerifyConnectionState())
                {
                    ErrorLog.AddError(ErrorType.Message, string.Format(Strings.WiFi_DisconnectedFromNetwork, networkName));

                    return;
                }
                else
                {
                    ErrorLog.AddError(ErrorType.Message, string.Format(Strings.WiFi_DisconnectedFromNetwork, networkName));
                }
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.WiFi_CantDisconnectFromNetwork);
                Logger.Write(ex);
            }
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new NetworkConfig(networkName, Strings.DisconnectNetwork_Title, Strings.WiFi_InformationTextDisconnectNetwork); }
        }
    }
}
