using System.Runtime.Serialization;
using System.Timers;
using ConnectivityModule.Wpf;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;

namespace ConnectivityModule.Events
{
    /// <summary>
    /// A class that detects when the monitored network becomes available.
    /// </summary>
    [DataContract]
    [MayhemModule("Wi-Fi: Network Becomes Available", "The selected network becomes available")]
    public class WiFiNetworkBecomesAvailable : WiFiEventBaseClass, IWpfConfigurable
    {
        /// <summary>
        /// This method is called when the timer.Elapsed event is raised and checks if the monitored network has become available since the last check.
        /// </summary>
        protected override void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();

            if (VerifyNetworkAvailability() && !wasAvailable && isAvailable)
            {
                Trigger();

                wasAvailable = true;
            }

            timer.Start();
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new NetworkAvailableConfig(networkName, seconds, Strings.NetworkBecomesAvailable_Title); }
        }
    }
}
