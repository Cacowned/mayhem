﻿using System.Runtime.Serialization;
using MayhemCore;
using NativeWifi;
using System.ComponentModel;
using System;

namespace ConnectivityModule.Reactions
{
    /// <summary>
    /// A class that connects to a predefined network.
    /// </summary>
    [DataContract]
    [MayhemModule("WiFi: Connect To Network", "Connects to a specific network")]
    public class WiFiConnectNetwork : WiFiReactionBaseClass
    {
        /// <summary>
        /// This method will try to connect to the predefined network.
        /// </summary>
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
