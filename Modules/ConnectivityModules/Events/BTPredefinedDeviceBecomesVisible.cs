using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Timers;
using MayhemCore;

namespace ConnectivityModule.Events
{
    /// <summary>
    /// A class that detects when a predefined bluetooth device becomes visible.
    /// </summary>
    [DataContract]
    [MayhemModule("Bluetooth: Predefined Device Visible", "Triggers when the selected device becomes visible")]
    public class BTPredefinedDeviceBecomesVisible : BTPredefinedDeviceBaseClass
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
    }
}
