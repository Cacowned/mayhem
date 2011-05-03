using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using MayhemCore;
using OOutlook = Microsoft.Office.Interop.Outlook;

namespace OfficeModules.Actions.Outlook
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
			// Create the event handler delegate to attach
			mailEvent = new OOutlook.ApplicationEvents_11_NewMailEventHandler(GotMail);
		}

		private void GotMail() {
			base.OnActionActivated();
		}

		public override void Enable() {

            outlook = OfficeFactory.GetOutlook();

            if (outlook != null)
            {
                base.Enable();

                outlook.NewMail += mailEvent;
            }
		}

		public override void Disable() {
			
            // TODO: Sometimes outlook is null here
            // how is that possible?
            if (outlook != null)
            {
                outlook.NewMail -= mailEvent;

                outlook = null;
            }

            base.Disable();
		}

		#region Serialization
		public OutlookMail(SerializationInfo info, StreamingContext context)
			: base(info, context) {

			SetUp();
		}
		#endregion
	}
}
