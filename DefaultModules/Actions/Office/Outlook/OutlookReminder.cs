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
			try {
				outlook = (OOutlook.Application)Marshal.GetActiveObject("Outlook.Application");
				reminderEvent = new OOutlook.ApplicationEvents_11_ReminderEventHandler(GotReminder);
			} catch (Exception e) {
				Debug.Write(e);
			}
		}

		private void GotReminder(object sender) {
			base.OnActionActivated();
		}


		public override void Enable() {
			base.Enable();

			outlook.Reminder += reminderEvent;

		}

		public override void Disable() {
			base.Disable();

			outlook.Reminder -= reminderEvent;
		}

		#region Serialization
		public OutlookReminder(SerializationInfo info, StreamingContext context)
			: base(info, context) {

			SetUp();
		}
		#endregion
	}
}
