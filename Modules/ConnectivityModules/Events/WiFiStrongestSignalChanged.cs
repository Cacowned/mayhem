using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Text;
using System.Timers;
using MayhemCore;
using NativeWifi;

namespace ConnectivityModule.Events
{
    [DataContract]
    [MayhemModule("Wi-Fi: Strongest Signal Changed", "The network with the strongest signal has changed")]
    public class WiFiStrongestSignalChanged : EventBase
    {
        private WlanClient client;
        private Wlan.WlanAvailableNetwork strongestNetwork;
        private Wlan.WlanAvailableNetwork newerStrongestNetwork;
        private Timer timer;

        private uint signalQualityStrongestNetwork;
        private uint newMaximumQualityValue;

        protected override void OnEnabling(EnablingEventArgs e)
        {
            timer = new Timer();
            timer.Interval = int.Parse(Strings.General_TimerInterval);
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);

            signalQualityStrongestNetwork = 0;

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
                        if (signalQualityStrongestNetwork < network.wlanSignalQuality)
                        {
                            strongestNetwork = network;
                            signalQualityStrongestNetwork = network.wlanSignalQuality;
                        }
                    }
                }

                timer.Start();
            }
            catch (Win32Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.WiFi_CantGetNetworks);
                Logger.Write(ex);

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
            try
            {
                foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
                {
                    newMaximumQualityValue = 0;

                    foreach (Wlan.WlanAvailableNetwork network in wlanIface.GetAvailableNetworkList(0))
                    {
                        if (GetStringForSSID(network.dot11Ssid).Equals(GetStringForSSID(strongestNetwork.dot11Ssid)) && network.wlanSignalQuality > signalQualityStrongestNetwork)
                        {
                            // Update value
                            signalQualityStrongestNetwork = network.wlanSignalQuality;
                        }
                        else
                        {
                            if (!GetStringForSSID(network.dot11Ssid).Equals(GetStringForSSID(strongestNetwork.dot11Ssid)) && newMaximumQualityValue < network.wlanSignalQuality)
                            {
                                newMaximumQualityValue = network.wlanSignalQuality;
                                newerStrongestNetwork = network;
                            }
                        }
                    }
                }

                if (newMaximumQualityValue > signalQualityStrongestNetwork)
                {
                    signalQualityStrongestNetwork = newMaximumQualityValue;
                    strongestNetwork = newerStrongestNetwork;

                    Trigger();
                }
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
    }
}
