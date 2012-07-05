using System;
using System.Runtime.InteropServices;
using MayhemCore;
using OfficeModules.Resources;
using OPowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace OfficeModules.Events.PowerPoint
{
    /// <summary>
    /// An event that will be triggered when a presentation is printed.
    /// </summary>
    [MayhemModule("PowerPoint: Print Presentation", "Triggers when a presentation is printed")]
    public class PptPrintPresentation : EventBase
    {
        OPowerPoint.Application powerPoint;
        OPowerPoint.EApplication_PresentationPrintEventHandler printPresentationEvent;

        /// <summary>
        /// This method is called after the event is loaded.
        /// </summary>
        protected override void OnAfterLoad()
        {
            // Create the event handler delegate to attach
            printPresentationEvent = PresentationPrinted;
        }

        /// <summary>
        /// This method is called when the EApplication_PresentationPrintEventHandler is triggered and will trigger this event.
        /// </summary>
        /// <param name="pres">The object representation of the current presentation</param>
        private void PresentationPrinted(OPowerPoint.Presentation pres)
        {
            Trigger();
        }

        /// <summary>
        /// This method gets the PowerPoint instance and is subscribing to the EApplication_PresentationPrintEventHandler.
        /// </summary>
        protected override void OnEnabling(EnablingEventArgs e)
        {
            // When enabled, try and get the PowerPoint instance
            try
            {
                powerPoint = (OPowerPoint.Application)Marshal.GetActiveObject("PowerPoint.Application");
                powerPoint.PresentationPrint += printPresentationEvent;
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.PowerPoint_ApplicationNotFound);
                Logger.Write(ex);
                e.Cancel = true;
            }
        }

        /// <summary>
        /// This method is unsubscribing from the EApplication_PresentationPrintEventHandler.
        /// </summary>
        protected override void OnDisabled(DisabledEventArgs e)
        {
            if (powerPoint != null)
            {
                powerPoint.PresentationPrint -= printPresentationEvent;
                powerPoint = null;
            }
        }
    }
}
