using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using ConnectivityModule.Wpf;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using NativeWifi;

namespace ConnectivityModule.Reactions
{
    [DataContract]
    [MayhemModule("WiFi: Connect To Network", "Connects to a specific network")]
    public class WiFiConnectNetwork : ReactionBase, IWpfConfigurable
    {
        [DataMember]
        private string networkName;

        private WlanClient client;

        public override void Perform()
        {
            client = new WlanClient();
            
            try
            {
                foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
                {
                    Wlan.WlanAvailableNetwork[] networks = wlanIface.GetAvailableNetworkList(0);
                    foreach (Wlan.WlanAvailableNetwork network in networks)
                    {
                        if (GetStringForSSID(network.dot11Ssid).Equals(networkName))
                        {
                            string profileXml = string.Format("<?xml version=\"1.0\"?><WLANProfile xmlns=\"http://www.microsoft.com/networking/WLAN/profile/v1\"><name>{0}</name><SSIDConfig><SSID><name>{0}</name></SSID><nonBroadcast>false</nonBroadcast></SSIDConfig><connectionType>ESS</connectionType><connectionMode>manual</connectionMode><MSM><security><authEncryption><authentication>open</authentication><encryption>none</encryption><useOneX>false</useOneX></authEncryption></security></MSM></WLANProfile>", networkName);

                            bool found = false;
                            foreach (Wlan.WlanProfileInfo profileInfo in wlanIface.GetProfiles())
                            {
                                if (profileInfo.profileName.Equals(GetStringForSSID(network.dot11Ssid)))
                                {
                                    found = true;
                                    break;
                                }
                            }

                            if (!found)
                                wlanIface.SetProfile(Wlan.WlanProfileFlags.AllUser, profileXml, true);

                            string command = "netsh wlan connect name=\"" + networkName + "\"";

                            ProcessStartInfo processInfo = new ProcessStartInfo("cmd.exe", "/C " + command);
                          
                            processInfo.WindowStyle = ProcessWindowStyle.Hidden;
                            Process.Start(processInfo);

                            found = false;

                            Thread.Sleep(2000);

                            foreach (WlanClient.WlanInterface wlanInterface in client.Interfaces)
                            {
                                try
                                {
                                    Wlan.WlanConnectionAttributes connAtributes = wlanInterface.CurrentConnection;

                                    if (connAtributes.profileName.Equals(networkName))
                                        found = true;
                                }
                                catch (Win32Exception ex)
                                {
                                    ErrorLog.AddError(ErrorType.Failure, Strings.WiFi_CantConnectToNetwork);
                                    Debug.Write(ex);
                                    return;
                                }
                            }

                            if (found == false)
                                ErrorLog.AddError(ErrorType.Failure, Strings.WiFi_CantConnectToNetwork);
                            else
                                ErrorLog.AddError(ErrorType.Message, string.Format(Strings.WiFi_SuccessConnectedNetwork, networkName));
                            
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.WiFi_CantConnectToNetwork);
                Debug.Write(ex);
            }
        }

        private string GetStringForSSID(Wlan.Dot11Ssid ssid)
        {
            return Encoding.ASCII.GetString(ssid.SSID, 0, (int)ssid.SSIDLength);
        }

        #region IWpfConfigurable Methods

        public WpfConfiguration ConfigurationControl
        {
            get { return new MonitorNetworkConfig(networkName); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = configurationControl as MonitorNetworkConfig;

            if (config == null)
                return;

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
