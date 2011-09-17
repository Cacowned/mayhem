using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using MayhemCore;
using OOutlook = Microsoft.Office.Interop.Outlook;
using OfficeModules.Resources;

namespace OfficeModules.Events
{
    [DataContract]
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
            base.OnEventActivated();
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

            outlook.Reminder -= reminderEvent;

            outlook = null;
        }
    }
}
