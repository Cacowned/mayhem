using System.Runtime.InteropServices;
using MayhemCore;
using Microsoft.Office.Interop.PowerPoint;
using OfficeModules.Resources;
using OPowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace OfficeModules.Events
{
    public class PptActivePresentationChanged : EventBase
    {
        [MayhemModule("PowerPoint: Active Presentation changed", "Triggers when the active presentation has changed")]
        public class PptOpenPresentation : EventBase
        {
            OPowerPoint.Application powerPoint;
            OPowerPoint.EApplication_WindowActivateEventHandler activePresentationChangedEvent;

            protected override void OnAfterLoad()
            {
                // Create the event handler delegate to attach
                activePresentationChangedEvent = ActivePresentationChanged;
            }

            private void ActivePresentationChanged(OPowerPoint.Presentation presentation, DocumentWindow window)
            {
                Trigger();
            }

            protected override void OnEnabling(EnablingEventArgs e)
            {
                // When enabled, try and get the PowerPoint instance
                try
                {
                    powerPoint = (OPowerPoint.Application)Marshal.GetActiveObject("PowerPoint.Application");
                    powerPoint.WindowActivate += activePresentationChangedEvent;
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
                    powerPoint.WindowActivate -= activePresentationChangedEvent;
                    powerPoint = null;
                }
            }
        }
    }
}
