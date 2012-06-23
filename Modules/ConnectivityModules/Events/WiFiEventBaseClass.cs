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
    public abstract class WiFiEventBaseClass : EventBase
    {
        [DataMember]
        protected string networkName;

        [DataMember]
        protected int seconds;

        protected WlanClient client;
        protected Timer timer;

        protected bool isAvailable;
        protected bool wasAvailable;

        protected override void OnEnabling(EnablingEventArgs e)
        {

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
                        if (GetStringForSSID(network.dot11Ssid).Equals(networkName))
                        {
                            isAvailable = true;
                            wasAvailable = true;

                            // If the network is already visible we should inform the user
                            ErrorLog.AddError(ErrorType.Message, string.Format(Strings.WiFi_NetworkAlreadyVisible, networkName));
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

        protected string GetStringForSSID(Wlan.Dot11Ssid ssid)
        {
            return Encoding.ASCII.GetString(ssid.SSID, 0, (int)ssid.SSIDLength);
        }

        protected bool InitializeTimerCreateClient()
        {
            timer = new Timer();
            timer.Interval = seconds * 1000;
            timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);

            isAvailable = false;
            wasAvailable = false;

            try
            {
                client = new WlanClient();

                return true;
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.WiFi_WiFiNotAvailable);
                Logger.Write(ex);

                return false;
            }
        }

        protected bool VerifyNetworkAvailability()
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
                        {
                            found = true; // The Network we search for is in the list of the available networks

                            if (!isAvailable)
                            {
                                wasAvailable = false;
                                isAvailable = true;
                            }

                            break;
                        }
                    }
                }

                if (!found && isAvailable)
                {
                    wasAvailable = false;
                    isAvailable = true;
                }

                return true;
            }
            catch (Win32Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.WiFi_CantGetNetworks);
                Logger.WriteLine(ex.Message);

                return false;
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.WiFi_CantGetNetworks);
                Logger.Write(ex);

                return false;
            }
        }

        protected abstract void timer_Elapsed(object sender, ElapsedEventArgs e);
    }
}
