using System.Globalization;
using System.Runtime.Serialization;
using System.Timers;
using ConnectivityModule.Wpf;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;

namespace ConnectivityModule.Events
{
    /// <summary>
    /// A class that detects when the monitored network is no longer available.
    /// </summary>
    [DataContract]
    [MayhemModule("Wi-Fi: Network No Longer Available", "The selected network is no longer available")]
    public class WiFiNetworkNoLongerAvailable : WiFiEventBaseClass, IWpfConfigurable
    {
        /// <summary>
        /// This method is called when the timer.Elapsed event is raised and checks if the monitored network is no longer available.
        /// </summary>
        protected override void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();

            // If the device was previously found but is no longer in reach we trigger the event.
            if (VerifyNetworkAvailability() && wasAvailable && !isAvailable)
            {
                Trigger();

                // We triggered the event so we'll wait for the next event.
                wasAvailable = false;
            }

            timer.Start();
        }

        #region IWpfConfigurable Methods

        public WpfConfiguration ConfigurationControl
        {
            get { return new NetworkNoLongerAvailableConfig(networkName, seconds); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = configurationControl as NetworkNoLongerAvailableConfig;

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
