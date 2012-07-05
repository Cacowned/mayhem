using System.Runtime.InteropServices;
using MayhemCore;
using OfficeModules.Resources;
using OOutlook = Microsoft.Office.Interop.Outlook;

namespace OfficeModules.Events
{
    /// <summary>
    /// An event that will be triggered when a reminder goes off for an Outlook event.
    /// </summary>
    [MayhemModule("Outlook Reminder", "Triggers when a reminder goes off for an Outlook event")]
    public class OutlookReminder : EventBase
    {
        private OOutlook.Application outlook;
        private OOutlook.ApplicationEvents_11_ReminderEventHandler reminderEvent;

        /// <summary>
        /// This method is called after the event is loaded.
        /// </summary>
        protected override void OnAfterLoad()
        {
            // Create the event handler delegate to attach
            reminderEvent = GotReminder;
        }

        /// <summary>
        /// This method is called when the ApplicationEvents_11_ReminderEventHandler is triggered and will trigger this event.
        /// </summary>
        private void GotReminder(object sender)
        {
            Trigger();
        }

        /// <summary>
        /// This method gets the Outlook instance and is subscribing to the ApplicationEvents_11_ReminderEventHandler.
        /// </summary>
        protected override void OnEnabling(EnablingEventArgs e)
        {
            // When enabled, try and get the outlook instance
            try
            {
                outlook = (OOutlook.Application)Marshal.GetActiveObject("Outlook.Application");
                outlook.Reminder += reminderEvent;
            }
            catch
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.Outlook_ApplicationNotFound);
                e.Cancel = true;
            }
        }

        /// <summary>
        /// This method is unsubscribing from the ApplicationEvents_11_ReminderEventHandler.
        /// </summary>
        protected override void OnDisabled(DisabledEventArgs e)
        {
            if (outlook != null)
            {
                outlook.Reminder -= reminderEvent;

                outlook = null;
            }
        }
    }
}
