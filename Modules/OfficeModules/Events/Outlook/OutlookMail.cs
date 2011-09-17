using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using MayhemCore;
using OOutlook = Microsoft.Office.Interop.Outlook;
using OfficeModules.Resources;

namespace OfficeModules.Events
{
    [MayhemModule("Outlook New Mail", "Triggers when a new email is received")]
    public class OutlookMail : EventBase
    {
        private OOutlook.Application outlook;
        private OOutlook.ApplicationEvents_11_NewMailEventHandler mailEvent;

        protected override void Initialize()
        {
            base.Initialize();

            // Create the event handler delegate to attach
            mailEvent = new OOutlook.ApplicationEvents_11_NewMailEventHandler(GotMail);
        }

        private void GotMail()
        {
            Trigger();
        }

        public override void Enable()
        {
            // When enabled, try and get the outlook instance            
            try
            {
                outlook = (OOutlook.Application)Marshal.GetActiveObject("Outlook.Application");
                outlook.NewMail += mailEvent;

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

            // TODO: Sometimes outlook is null here
            // how is that possible?
            if (outlook != null)
            {
                outlook.NewMail -= mailEvent;

                outlook = null;
            }
        }
    }
}
