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

        protected override void OnAfterLoad()
        {
            savePresentationEvent = PresentationSaved;
        }

        private void PresentationSaved(OPowerPoint.Presentation pres)
        {
            Trigger();
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
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
