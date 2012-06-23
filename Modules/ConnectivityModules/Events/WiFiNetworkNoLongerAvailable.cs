﻿using System.Globalization;
using System.Runtime.Serialization;
using System.Timers;
using ConnectivityModule.Wpf;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;

namespace ConnectivityModule.Events
{
    [DataContract]
    [MayhemModule("Wi-Fi: Network No Longer Available", "The selected network is no longer available")]
    public class WiFiNetworkNoLongerAvailable : WiFiEventBaseClass, IWpfConfigurable
    {
        protected override void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();

            // If the device was previously found but is no longer in reach we trigger the event
            if (VerifyNetworkAvailability() && wasAvailable && !isAvailable)
            {
                Trigger();

                wasAvailable = false; // We triggered the event so we'll wait for the next event
            }

            timer.Start();
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
