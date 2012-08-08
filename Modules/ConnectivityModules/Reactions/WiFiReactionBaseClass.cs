﻿using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using ConnectivityModule.Wpf;
using MayhemCore;
using MayhemWpf.UserControls;
using NativeWifi;

namespace ConnectivityModule.Reactions
{
    /// <summary>
    /// An abstract base class that contains common code for the reactions that connect or disconnect to/from a network.
    /// </summary>
    [DataContract]
    public abstract class WiFiReactionBaseClass : ReactionBase
    {
        [DataMember]
        protected string networkName;

        protected WlanClient client;

        protected override void OnAfterLoad()
        {
            client = null;
        }

        protected bool ChangeConnectivityMode(string command)
        {
            try
            {
                if (client == null)
                    client = new WlanClient();
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.WiFi_WiFiNotAvailable);
                Logger.Write(ex);

                return false;
            }

            foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
            {
                Wlan.WlanAvailableNetwork[] networks = wlanIface.GetAvailableNetworkList(0);
                foreach (Wlan.WlanAvailableNetwork network in networks)
                {
                    if (network.profileName.Equals(networkName) || (network.profileName.Equals("") && networkName.Equals(GetStringForSSID(network.dot11Ssid))))
                    {
                        string profileXml = string.Format(@"<?xml version=""1.0""?>
<WLANProfile xmlns=""http://www.microsoft.com/networking/WLAN/profile/v1"">
    <name>{0}</name>
    <SSIDConfig>
        <SSID>
            <name>{0}</name>
        </SSID>
            <nonBroadcast>false</nonBroadcast>
     </SSIDConfig>
     <connectionType>ESS</connectionType>
     <connectionMode>manual</connectionMode>
    <MSM>
        <security>
            <authEncryption>
                <authentication>open</authentication>
                <encryption>none</encryption>
                <useOneX>false</useOneX>
            </authEncryption>
        </security>
    </MSM>
</WLANProfile>", networkName);

                        bool found = false;
                        foreach (Wlan.WlanProfileInfo profileInfo in wlanIface.GetProfiles())
                        {
                            if (profileInfo.profileName.Equals(network.profileName))
                            {
                                found = true;

                                break;
                            }
                        }

                        if (!found)
                            wlanIface.SetProfile(Wlan.WlanProfileFlags.AllUser, profileXml, true);

                        ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe", "/C " + command);

                        processInfo.WindowStyle = ProcessWindowStyle.Hidden;
                        Process.Start(processInfo);

                        Thread.Sleep(2000); // We wait for the connect/disconnect command to finish.

                        return true;
                    }
                }
            }

            // If the desired network wasn't found we assume is not in reach.
            ErrorLog.AddError(ErrorType.Failure, string.Format(Strings.WiFi_NetworkNotVisible, networkName));

            return false;
        }

        protected string GetStringForSSID(Wlan.Dot11Ssid ssid)
        {
            return Encoding.ASCII.GetString(ssid.SSID, 0, (int)ssid.SSIDLength);
        }

        public abstract override void Perform();

        #region IWpfConfigurable Methods

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = configurationControl as NetworkConfig;

            if (config == null)
            {
                return;
            }

            networkName = config.NetworkName;
        }

        #endregion

        #region IWpfConfigurable Members

        public string GetConfigString()
        {
            return string.Format(CultureInfo.CurrentCulture, Strings.NetworkName_ConfigString, networkName);
        }

        #endregion
    }
}
