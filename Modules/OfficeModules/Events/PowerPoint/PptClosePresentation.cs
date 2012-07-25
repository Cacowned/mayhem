using System;
using System.Runtime.InteropServices;
using MayhemCore;
using OfficeModules.Resources;
using OPowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace OfficeModules.Events.PowerPoint
{
    /// <summary>
    /// An event that will be triggered when a presentation is closed.
    /// </summary>
    [MayhemModule("PowerPoint: Close Presentation", "Triggers when a presentation is closed")]
    public class PptClosePresentation : EventBase
    {
        OPowerPoint.Application powerPoint;
        OPowerPoint.EApplication_PresentationCloseFinalEventHandler closePresentationEvent;

        protected override void OnAfterLoad()
        {
            closePresentationEvent = PresentationClosed;
        }

        private void PresentationClosed(OPowerPoint.Presentation pres)
        {
            Trigger();
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
            try
            {
                powerPoint = (OPowerPoint.Application)Marshal.GetActiveObject("PowerPoint.Application");
                powerPoint.PresentationCloseFinal += closePresentationEvent;
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
                powerPoint.PresentationCloseFinal -= closePresentationEvent;
                powerPoint = null;
            }
        }
    }
}
