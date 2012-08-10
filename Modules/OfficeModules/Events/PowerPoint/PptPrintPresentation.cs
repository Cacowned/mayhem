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

        protected override void OnAfterLoad()
        {
            printPresentationEvent = PresentationPrinted;
        }

        private void PresentationPrinted(OPowerPoint.Presentation pres)
        {
            Trigger();
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
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
