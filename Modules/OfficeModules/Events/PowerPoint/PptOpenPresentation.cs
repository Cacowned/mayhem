using System;
using System.Runtime.InteropServices;
using MayhemCore;
using OfficeModules.Resources;
using OPowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace OfficeModules.Events
{
    [MayhemModule("PowerPoint: Open Presentation", "Triggers when a presentation is opened")]
    public class PptOpenPresentation : EventBase
    {
        OPowerPoint.Application powerPoint;
        OPowerPoint.EApplication_AfterPresentationOpenEventHandler openPresentationEvent;

        protected override void OnAfterLoad()
        {
            // Create the event handler delegate to attach
            openPresentationEvent = PresentationOpened;
        }

        private void PresentationOpened(OPowerPoint.Presentation pres)
        {
            Trigger();
        }

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
