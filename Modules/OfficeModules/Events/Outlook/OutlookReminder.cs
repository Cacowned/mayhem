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

        protected override void OnAfterLoad()
        {
            reminderEvent = GotReminder;
        }

        private void GotReminder(object sender)
        {
            Trigger();
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
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
