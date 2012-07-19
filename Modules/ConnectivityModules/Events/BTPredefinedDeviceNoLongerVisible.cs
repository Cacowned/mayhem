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
    /// A class that detects when a predefined bluetooth device is no longer visible.
    /// </summary>
    [DataContract]
    [MayhemModule("Bluetooth: Predefined Device No Longer Visible", "Triggers when the selected device is no longer visible")]
    public class BTPredefinedDeviceNoLongerVisible : BTPredefinedDeviceBaseClass, IWpfConfigurable
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected override void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();

            // If the device was previously found but is no longer in reach we'll trigger the event.
            if (VerifyDeviceVisibility() && wasVisible && !isVisible)
            {
                Trigger();

                // We triggered the event so we'll wait for the next event.
                wasVisible = false;
            }

            timer.Start();
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new PredefinedDeviceVisibleConfig(deviceName, seconds, Strings.PredefinedDeviceNoLongerVisible_Title, Strings.BT_DeviceTypeTextName, Strings.BT_InformationTextPredefinedDeviceVisible); }
        }
    }
}
