using System.Runtime.InteropServices;
using MayhemCore;
using OfficeModules.Resources;
using OPowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace OfficeModules.Events.PowerPoint
{
    [MayhemModule("PowerPoint: Print Presentation", "Triggers when a presentation is printed")]
    public class PptPrintPresentation : EventBase
    {
        OPowerPoint.Application powerPoint;
        OPowerPoint.EApplication_PresentationPrintEventHandler printPresentationEvent;

        protected override void OnAfterLoad()
        {
            // Create the event handler delegate to attach
            printPresentationEvent = PresentationPrinted;
        }

        private void PresentationPrinted(OPowerPoint.Presentation pres)
        {
            Trigger();
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
            // When enabled, try and get the PowerPoint instance
            try
            {
                powerPoint = (OPowerPoint.Application)Marshal.GetActiveObject("PowerPoint.Application");
                powerPoint.PresentationPrint += printPresentationEvent;
            }
            catch
            {
                ErrorLog.AddError(ErrorType.Warning, Strings.PowerPoint_ApplicationNotFound);
                e.Cancel = true;
            }
        }

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
