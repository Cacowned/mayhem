﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using MayhemCore;
using OOutlook = Microsoft.Office.Interop.Outlook;

namespace OfficeModules.Actions.Outlook
{
    [Serializable]
    public class OutlookReminder : ActionBase, ISerializable
    {
        protected OOutlook.Application outlook;
        protected OOutlook.ApplicationEvents_11_ReminderEventHandler reminderEvent;

        public OutlookReminder()
            : base("Outlook Reminder", "Triggers when a reminder goes off for an outlook event")
        {

            SetUp();
        }

        protected void SetUp()
        {
            // Create the event handler delegate to attach
            reminderEvent = new OOutlook.ApplicationEvents_11_ReminderEventHandler(GotReminder);
        }

        private void GotReminder(object sender)
        {
            base.OnActionActivated();
        }


        public override void Enable()
        {
            outlook = OfficeFactory.GetOutlook();

            if (outlook != null)
            {
                base.Enable();
                outlook.Reminder += reminderEvent;
            }
        }

        public override void Disable()
        {
            base.Disable();

            outlook.Reminder -= reminderEvent;

            outlook = null;
        }

        #region Serialization
        public OutlookReminder(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

            SetUp();
        }
        #endregion
    }
}
