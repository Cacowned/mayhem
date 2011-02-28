﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using MayhemCore;
using OOutlook = Microsoft.Office.Interop.Outlook;

namespace DefaultModules.Actions.Office.Outlook
{
	[Serializable]
	public class OutlookMail : ActionBase, ISerializable
	{
		protected OOutlook.Application outlook;
		protected OOutlook.ApplicationEvents_11_NewMailEventHandler mailEvent;

		public OutlookMail()
			: base("Outlook New Mail", "Triggers when a new email is received") {

			SetUp();
		}

		protected void SetUp() {

			try {
				outlook = (OOutlook.Application)Marshal.GetActiveObject("Outlook.Application");
				mailEvent = new OOutlook.ApplicationEvents_11_NewMailEventHandler(GotMail);
			} catch (Exception e) {
				Debug.Write(e);
			}
		}

		private void GotMail() {
			base.OnActionActivated();
		}

		public override void Enable() {
			base.Enable();

			outlook.NewMail += mailEvent;
		}

		public override void Disable() {
			base.Disable();

			outlook.NewMail -= mailEvent;
		}

		#region Serialization
		public OutlookMail(SerializationInfo info, StreamingContext context)
			: base(info, context) {

			SetUp();
		}
		#endregion
	}
}
