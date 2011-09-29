using System.Runtime.InteropServices;
using MayhemCore;
using OfficeModules.Resources;
using OOutlook = Microsoft.Office.Interop.Outlook;

namespace OfficeModules.Events
{
    [MayhemModule("Outlook New Mail", "Triggers when a new email is received")]
    public class OutlookMail : EventBase
    {
        private OOutlook.Application outlook;
        private OOutlook.ApplicationEvents_11_NewMailEventHandler mailEvent;

        protected override void Initialize()
        {
            // Create the event handler delegate to attach
            mailEvent = new OOutlook.ApplicationEvents_11_NewMailEventHandler(GotMail);
        }

        private void GotMail()
        {
            Trigger();
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
            // When enabled, try and get the outlook instance            
            try
            {
                outlook = (OOutlook.Application)Marshal.GetActiveObject("Outlook.Application");
                outlook.NewMail += mailEvent;
            }
            catch
            {
                ErrorLog.AddError(ErrorType.Warning, Strings.Outlook_ApplicationNotFound);
                e.Cancel = true;
            }
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            if (outlook != null)
            {
                outlook.NewMail -= mailEvent;

                outlook = null;
            }
        }
    }
}
