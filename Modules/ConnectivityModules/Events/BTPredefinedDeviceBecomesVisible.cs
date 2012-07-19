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
        [MethodImpl(MethodImplOptions.Synchronized)]
        protected override void timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            timer.Stop();

            // The event will be triggered if the VerifyDeviceVisibility method completed successfully and the bluetooth device wasn't visible and now is.
            if (VerifyDeviceVisibility() && !wasVisible && isVisible)
            {
                Trigger();

                wasVisible = true;
            }

            timer.Start();
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new PredefinedDeviceVisibleConfig(deviceName, seconds, Strings.PredefinedDeviceBecomesVisible_Title, Strings.BT_DeviceTypeTextName, Strings.BT_InformationTextPredefinedDeviceVisible); }
        }
    }
}
