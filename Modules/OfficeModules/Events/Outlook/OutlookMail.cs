using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using MayhemCore;
using OOutlook = Microsoft.Office.Interop.Outlook;

namespace OfficeModules.Events.Outlook
{
    [DataContract]
    [MayhemModule("Outlook New Mail", "Triggers when a new email is received")]
    public class OutlookMail : EventBase
    {
        protected OOutlook.Application outlook;
        protected OOutlook.ApplicationEvents_11_NewMailEventHandler mailEvent;

        protected override void Initialize()
        {
            base.Initialize();

            // Create the event handler delegate to attach
            mailEvent = new OOutlook.ApplicationEvents_11_NewMailEventHandler(GotMail);
        }

        private void GotMail()
        {
            base.OnEventActivated();
        }

        public override void Enable()
        {
            // When enabled, try and get the outlook instance            
            try
            {
                outlook = (OOutlook.Application)Marshal.GetActiveObject("Outlook.Application");
            }
            catch
            {
                ErrorLog.AddError(ErrorType.Warning, "Unable to find the open Outlook application");
                return;
            }

            base.Enable();

            outlook.NewMail += mailEvent;
        }

        public override void Disable()
        {

            // TODO: Sometimes outlook is null here
            // how is that possible?
            if (outlook != null)
            {
                outlook.NewMail -= mailEvent;

                outlook = null;
            }

            base.Disable();
        }
    }
}
