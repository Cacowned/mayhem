using System;
using System.Runtime.InteropServices;
using MayhemCore;
using OfficeModules.Resources;
using OPowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace OfficeModules.Events
{
    /// <summary>
    /// An event that will be triggered when a presentation is opened.
    /// </summary>
    [MayhemModule("PowerPoint: Open Presentation", "Triggers when a presentation is opened")]
    public class PptOpenPresentation : EventBase
    {
        OPowerPoint.Application powerPoint;
        OPowerPoint.EApplication_AfterPresentationOpenEventHandler openPresentationEvent;

        /// <summary>
        /// This method is called after the event is loaded.
        /// </summary>
        protected override void OnAfterLoad()
        {
            // Create the event handler delegate to attach
            openPresentationEvent = PresentationOpened;
        }

        /// <summary>
        /// This method is called when the EApplication_AfterPresentationOpenEventHandler is triggered and will trigger this event.
        /// </summary>
        private void PresentationOpened(OPowerPoint.Presentation pres)
        {
            Trigger();
        }

        /// <summary>
        /// This method gets the PowerPoint instance and is subscribing to the EApplication_AfterPresentationOpenEventHandler.
        /// </summary>
        protected override void OnEnabling(EnablingEventArgs e)
        {
            // When enabled, try and get the PowerPoint instance
            try
            {
                powerPoint = (OPowerPoint.Application)Marshal.GetActiveObject("PowerPoint.Application");
                powerPoint.AfterPresentationOpen += openPresentationEvent;
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.PowerPoint_ApplicationNotFound);
                Logger.Write(ex);
                e.Cancel = true;
            }
        }

        /// <summary>
        /// This method is unsubscribing from the EApplication_AfterPresentationOpenEventHandler.
        /// </summary>
        protected override void OnDisabled(DisabledEventArgs e)
        {
            if (powerPoint != null)
            {
                powerPoint.AfterPresentationOpen -= openPresentationEvent;
                powerPoint = null;
            }
        }
    }
}
