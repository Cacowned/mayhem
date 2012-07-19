using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text;
using System.Timers;
using ConnectivityModule.Wpf;
using MayhemCore;
using MayhemWpf.UserControls;
using NativeWifi;

namespace ConnectivityModule.Events
{
    /// <summary>
    /// An abstract base class that contains common code for the events that monitor the activity of a network.
    /// </summary>
    [DataContract]
    public abstract class WiFiEventBaseClass : EventBase
    {
        [DataMember]
        protected string networkName;

        /// <summary>
        /// The number of seconds to wait between checks.
        /// </summary>
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
                        if (network.profileName.Equals(networkName))
                        {
                            isAvailable = true;
                            wasAvailable = true;

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

        /// <summary>
        /// This method initializes the needed objects and if the WiFi capability is not available displays an error message.
        /// </summary>
        /// <returns>Returns true if the initialization finished successfully.</returns>
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
                // Displaing an error message If the WiFi capability is not available.
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

                // Cheking if the monitored Network is available.
                foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
                {
                    foreach (Wlan.WlanAvailableNetwork network in wlanIface.GetAvailableNetworkList(0))
                    {
                        if (network.profileName.Equals(networkName))
                        {
                            // The Network we search for is in the list of the available networks.
                            found = true;

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
                    wasAvailable = true;
                    isAvailable = false;
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

        #region IWpfConfigurable Methods

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = configurationControl as NetworkAvailableConfig;

            if (config == null)
            {
                return;
            }

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
