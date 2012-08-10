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

        public WpfConfiguration ConfigurationControl
        {
            get { return new NetworkAvailableConfig(networkName, seconds, Strings.NetworkNoLongerAvailable_Title); }
        }
    }
}
