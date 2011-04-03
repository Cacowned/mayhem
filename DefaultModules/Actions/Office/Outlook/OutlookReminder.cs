using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using MayhemCore;
using OOutlook = Microsoft.Office.Interop.Outlook;

namespace DefaultModules.Actions.Office.Outlook
{
	[Serializable]
	public class OutlookReminder : ActionBase, ISerializable
	{
		protected OOutlook.Application outlook;
		protected OOutlook.ApplicationEvents_11_ReminderEventHandler reminderEvent;

		public OutlookReminder()
			: base("Outlook Reminder", "Triggers when a reminder goes off for an outlook event") {

			SetUp();
		}

		protected void SetUp() {
			// Create the event handler delegate to attach
			reminderEvent = new OOutlook.ApplicationEvents_11_ReminderEventHandler(GotReminder);
		}

		private void GotReminder(object sender) {
			base.OnActionActivated();
		}


		public override void Enable() {
			

			// When enabled, try and get the outlook instance
			try {
				outlook = (OOutlook.Application)Marshal.GetActiveObject("Outlook.Application");
			} catch (Exception e) {
                ErrorLog.AddError(ErrorType.Warning, "Unable to find the open Outlook application");
                return;
			}

            base.Enable();

			outlook.Reminder += reminderEvent;

		}

		public override void Disable() {
			base.Disable();

			outlook.Reminder -= reminderEvent;

			outlook = null;
		}

		#region Serialization
		public OutlookReminder(SerializationInfo info, StreamingContext context)
			: base(info, context) {

			SetUp();
		}
		#endregion
	}
}
