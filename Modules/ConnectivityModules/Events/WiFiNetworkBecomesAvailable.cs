using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using System.Timers;
using ConnectivityModule.Wpf;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using NativeWifi;

namespace ConnectivityModule.Events
{
    [DataContract]
    [MayhemModule("Wi-Fi: Network Becomes Available", "The selected network becomes available")]
    public class WiFiNetworkBecomesAvailable : EventBase, IWpfConfigurable
    {
        [DataMember]
        private string networkName;

        [DataMember]
        private int seconds;

        private WlanClient client;
        private Timer timer;
        private bool isAvailable;

        protected override void OnEnabling(EnablingEventArgs e)
        {
            timer = new Timer();
            timer.Interval = seconds * 1000;
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);

            isAvailable = false;

            try
            {
                client = new WlanClient();
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.WiFi_WiFiNotAvailable);
                Logger.Write(ex);

                e.Cancel = true;

                return;
            }

            try
            {
                foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
                {
                    foreach (Wlan.WlanAvailableNetwork network in wlanIface.GetAvailableNetworkList(0))
                    {
                        if (GetStringForSSID(network.dot11Ssid).Equals(networkName))
                        {
                            isAvailable = true;

                            // If the network is already visible we should inform the user
                            ErrorLog.AddError(ErrorType.Message, string.Format(Strings.WiFi_NetworkVisible, networkName));
                            break;
                        }
                    }
                }

                timer.Start();
            }
            catch (Win32Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.WiFi_CantGetNetworks);
                Logger.WriteLine(ex.Message);

                e.Cancel = true;
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.WiFi_CantGetNetworks);
                Logger.Write(ex);

                e.Cancel = true;
            }
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            if (timer != null)
            {
                timer.Stop();
                timer.Elapsed -= timer_Elapsed;
                timer.Dispose();
            }
        }

        private string GetStringForSSID(Wlan.Dot11Ssid ssid)
        {
            return Encoding.ASCII.GetString(ssid.SSID, 0, (int)ssid.SSIDLength);
        }

        private void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            bool found;
            try
            {
                found = false;
                foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
                {
                    foreach (Wlan.WlanAvailableNetwork network in wlanIface.GetAvailableNetworkList(0))
                    {
                        if (GetStringForSSID(network.dot11Ssid).Equals(networkName))
                            found = true; // The Network we search for is in the list of the available networks

                        if (found && isAvailable == false)
                        {
                            isAvailable = true;
                            Trigger();

                            break;
                        }
                    }
                }

                if (!found)
                    isAvailable = false;
            }
            catch (Win32Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.WiFi_CantGetNetworks);
                Logger.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.WiFi_CantGetNetworks);
                Logger.Write(ex);
            }
        }

        #region IWpfConfigurable Methods

        public WpfConfiguration ConfigurationControl
        {
            get { return new NetworkNameConfig(networkName, seconds); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = configurationControl as NetworkNameConfig;

            if (config == null)
                return;

            networkName = config.NetworkName;
            seconds = config.Seconds;
        }

        #endregion

        #region IConfigurable Members

        public string GetConfigString()
        {
            return string.Format(CultureInfo.CurrentCulture, Strings.NetworkName_ConfigString, networkName);
        }

        #endregion
    }
}
