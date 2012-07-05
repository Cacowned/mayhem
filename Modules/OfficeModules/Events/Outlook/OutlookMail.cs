using System.Runtime.InteropServices;
using MayhemCore;
using OfficeModules.Resources;
using OOutlook = Microsoft.Office.Interop.Outlook;

namespace OfficeModules.Events
{
    /// <summary>
    /// An event that will be triggered when a new email is received.
    /// </summary>
    [MayhemModule("Outlook New Mail", "Triggers when a new email is received")]
    public class OutlookMail : EventBase
    {
        private OOutlook.Application outlook;
        private OOutlook.ApplicationEvents_11_NewMailEventHandler mailEvent;

        /// <summary>
        /// This method is called after the event is loaded.
        /// </summary>
        protected override void OnAfterLoad()
        {
            // Create the event handler delegate to attach
            mailEvent = GotMail;
        }

        /// <summary>
        /// This method is called when the ApplicationEvents_11_NewMailEventHandler is triggered and will trigger this event.
        /// </summary>
        private void GotMail()
        {
            Trigger();
        }

        /// <summary>
        /// This method gets the Outlook instance and is subscribing to the ApplicationEvents_11_NewMailEventHandler.
        /// </summary>
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
                ErrorLog.AddError(ErrorType.Failure, Strings.Outlook_ApplicationNotFound);
                e.Cancel = true;
            }
        }

        /// <summary>
        /// This method is unsubscribing from the ApplicationEvents_11_NewMailEventHandler.
        /// </summary>
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
