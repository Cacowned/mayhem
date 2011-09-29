using System.Runtime.InteropServices;
using MayhemCore;
using OfficeModules.Resources;
using OOutlook = Microsoft.Office.Interop.Outlook;

namespace OfficeModules.Events
{
    [MayhemModule("Outlook Reminder", "Triggers when a reminder goes off for an outlook event")]
    public class OutlookReminder : EventBase
    {
        private OOutlook.Application outlook;
        private OOutlook.ApplicationEvents_11_ReminderEventHandler reminderEvent;

        protected override void Initialize()
        {
            // Create the event handler delegate to attach
            reminderEvent = new OOutlook.ApplicationEvents_11_ReminderEventHandler(GotReminder);
        }

        private void GotReminder(object sender)
        {
            Trigger();
        }

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
                ErrorLog.AddError(ErrorType.Warning, Strings.Outlook_ApplicationNotFound);
                e.Cancel = true;
            }
        }

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
