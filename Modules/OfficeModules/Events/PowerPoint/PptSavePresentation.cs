using System;
using System.Runtime.InteropServices;
using MayhemCore;
using OfficeModules.Resources;
using OPowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace OfficeModules.Events.PowerPoint
{
    /// <summary>
    /// An event that will be triggered when a presentation is saved.
    /// </summary>
    [MayhemModule("PowerPoint: Save Presentation", "Triggers when a presentation is saved")]
    public class PptSavePresentation : EventBase
    {
        OPowerPoint.Application powerPoint;
        OPowerPoint.EApplication_PresentationSaveEventHandler savePresentationEvent;

        /// <summary>
        /// This method is called after the event is loaded.
        /// </summary>
        protected override void OnAfterLoad()
        {
            // Create the event handler delegate to attach
            savePresentationEvent = PresentationSaved;
        }

        /// <summary>
        /// This method is called when the EApplication_PresentationSaveEventHandler is triggered and will trigger this event.
        /// </summary>
        /// <param name="pres">The object representation of the current presentation</param>
        private void PresentationSaved(OPowerPoint.Presentation pres)
        {
            Trigger();
        }

        /// <summary>
        /// This method gets the PowerPoint instance and is subscribing to the EApplication_PresentationSaveEventHandler.
        /// </summary>
        protected override void OnEnabling(EnablingEventArgs e)
        {
            // When enabled, try and get the PowerPoint instance
            try
            {
                powerPoint = (OPowerPoint.Application)Marshal.GetActiveObject("PowerPoint.Application");
                powerPoint.PresentationSave += savePresentationEvent;
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.PowerPoint_ApplicationNotFound);
                Logger.Write(ex);
                e.Cancel = true;
            }
        }

        /// <summary>
        /// This method is unsubscribing from the EApplication_PresentationSaveEventHandler.
        /// </summary>
        protected override void OnDisabled(DisabledEventArgs e)
        {
            if (powerPoint != null)
            {
                powerPoint.PresentationSave -= savePresentationEvent;
                powerPoint = null;
            }
        }
    }
}
