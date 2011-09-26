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

        public override bool Enable()
        {
            // When enabled, try and get the outlook instance
            try
            {
                outlook = (OOutlook.Application)Marshal.GetActiveObject("Outlook.Application");
                outlook.Reminder += reminderEvent;

                return true;
            }
            catch
            {
                ErrorLog.AddError(ErrorType.Warning, Strings.Outlook_ApplicationNotFound);
            }

            return false;
        }

        public override void Disable()
        {
            if (outlook != null)
            {
                outlook.Reminder -= reminderEvent;

                outlook = null;
            }
        }
    }
}
