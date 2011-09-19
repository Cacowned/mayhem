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
            base.Initialize();

            // Create the event handler delegate to attach
            reminderEvent = new OOutlook.ApplicationEvents_11_ReminderEventHandler(GotReminder);
        }

        private void GotReminder(object sender)
        {
            Trigger();
        }

        public override void Enable()
        {
            // When enabled, try and get the outlook instance
            try
            {
                outlook = (OOutlook.Application)Marshal.GetActiveObject("Outlook.Application");
                outlook.Reminder += reminderEvent;

                base.Enable();
            }
            catch
            {
                ErrorLog.AddError(ErrorType.Warning, Strings.Outlook_ApplicationNotFound);
            }
        }

        public override void Disable()
        {
            base.Disable();

            if (outlook != null)
            {
                outlook.Reminder -= reminderEvent;

                outlook = null;
            }
        }
    }
}
