using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using System.Timers;
using MayhemCore;
using NativeWifi;

namespace ConnectivityModule.Events
{
    /// <summary>
    /// A class that detects when the network with the strongest signal changed.
    /// </summary>
    [DataContract]
    [MayhemModule("Wi-Fi: Strongest Signal Changed", "The network with the strongest signal has changed")]
    public class WiFiStrongestSignalChanged : WiFiEventBaseClass
    {
        private Wlan.WlanAvailableNetwork strongestNetwork;
        private Wlan.WlanAvailableNetwork newerStrongestNetwork;

        private uint signalQualityStrongestNetwork;
        private uint newMaximumQualityValue;

        /// <summary>
        /// This method is initializing the needed objects and starts the timer.
        /// It also gets the network with the strongest signal.
        /// </summary>
        protected override void OnEnabling(EnablingEventArgs e)
        {
            // Setting the number of seconds
            seconds = int.Parse(Strings.General_TimerInterval) / 1000;

            if (!InitializeTimerCreateClient())
            {
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

        /// <summary>
        /// This method is called when the timer.Elapsed event is raised and checks if the network with the strongest signal has changed since the last check.
        /// </summary>
        protected override void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();

            try
            {
                bool found = false;
                foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
                {
                    newMaximumQualityValue = 0;

                    foreach (Wlan.WlanAvailableNetwork network in wlanIface.GetAvailableNetworkList(0))
                    {
                        if (GetStringForSSID(network.dot11Ssid).Equals(GetStringForSSID(strongestNetwork.dot11Ssid)))
                        {
                            found = true;
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

                    if (found == false)
                    {
                        signalQualityStrongestNetwork = 0; // The network is no longer available
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

            timer.Start();
        }
    }
}
