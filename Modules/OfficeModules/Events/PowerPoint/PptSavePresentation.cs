using System.Runtime.InteropServices;
using MayhemCore;
using OfficeModules.Resources;
using OPowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace OfficeModules.Events.PowerPoint
{
    [MayhemModule("PowerPoint: Save Presentation", "Triggers when a presentation is saved")]
    public class PptSavePresentation : EventBase
    {
        OPowerPoint.Application powerPoint;
        OPowerPoint.EApplication_PresentationSaveEventHandler savePresentationEvent;

        protected override void OnAfterLoad()
        {
            // Create the event handler delegate to attach
            savePresentationEvent = PresentationSaved;
        }

        private void PresentationSaved(OPowerPoint.Presentation pres)
        {
            Trigger();
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
            // When enabled, try and get the PowerPoint instance
            try
            {
                powerPoint = (OPowerPoint.Application)Marshal.GetActiveObject("PowerPoint.Application");
                powerPoint.PresentationSave += savePresentationEvent;
            }
            catch
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.PowerPoint_ApplicationNotFound);
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
