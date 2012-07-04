using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Timers;
using ConnectivityModule.Wpf;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;

namespace ConnectivityModule.Events
{
    /// <summary>
    /// A class that detects when a predefined bluetooth device becomes visible.
    /// </summary>
    [DataContract]
    [MayhemModule("Bluetooth: Predefined Device Becomes Visible", "Triggers when the selected device becomes visible")]
    public class BTPredefinedDeviceBecomesVisible : BTPredefinedDeviceBaseClass, IWpfConfigurable
    {
        /// <summary>
        /// This method will set the string representing the type of event this class will trigger: Strings.BT_MonitorBecomesVisible.
        /// </summary>
        protected override void OnAfterLoad()
        {
            monitorType = Strings.BT_MonitorBecomesVisible;
        }

        /// <summary>
        /// This method is called when the timer.Elapsed event is raised and checks if the predefined bluetooth device has become visible since the last check.
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected override void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Stopping the timer so this event will not be triggered again until the method exits.
            timer.Stop();

            // The event will be triggered if the VerifyDeviceVisibility method completed successfully and the bluetooth device wasn't visible and now is.
            if (VerifyDeviceVisibility() && !wasVisible && isVisible)
            {
                Trigger();

                wasVisible = true;
            }

            // Starting the timer.
            timer.Start();
        }

        #region IWpfConfigurable Methods

        public WpfConfiguration ConfigurationControl
        {
            get { return new PredefinedDeviceBecomesVisibleConfig(deviceName, seconds); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = configurationControl as PredefinedDeviceBecomesVisibleConfig;

            if (config == null)
            {
                return;
            }

            deviceName = config.DeviceName;
            seconds = config.Seconds - 5; // We wait at least 5 seconds for the DiscoverDevices() method so we deduct that time from the total wait time.

            if (seconds <= 0)
            {
                seconds = 1; // We need to wait at least 1 second.
            }
        }

        #endregion

        #region IConfigurable Members

        public string GetConfigString()
        {
            return string.Format(CultureInfo.CurrentCulture, Strings.DeviceName_ConfigString, deviceName, monitorType);
        }

        #endregion
    }
}
