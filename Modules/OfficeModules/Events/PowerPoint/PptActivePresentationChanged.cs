using System;
using System.Runtime.InteropServices;
using MayhemCore;
using Microsoft.Office.Interop.PowerPoint;
using OfficeModules.Resources;
using OPowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace OfficeModules.Events
{
    /// <summary>
    /// An event that will be triggered when the active presentation changes.
    /// </summary>
    public class PptActivePresentationChanged : EventBase
    {
        [MayhemModule("PowerPoint: Active Presentation Changed", "Triggers when the active presentation has changed")]
        public class PptOpenPresentation : EventBase
        {
            OPowerPoint.Application powerPoint;
            OPowerPoint.EApplication_WindowActivateEventHandler activePresentationChangedEvent;

            /// <summary>
            /// This method is called after the event is loaded.
            /// </summary>
            protected override void OnAfterLoad()
            {
                // Create the event handler delegate to attach
                activePresentationChangedEvent = ActivePresentationChanged;
            }

            /// <summary>
            /// This method is called when the EApplication_WindowActivateEventHandle is triggered and will trigger this event.
            /// </summary>
            /// <param name="presentation">The object representation of the current presentation</param>
            /// <param name="window">The window of the current presentation</param>
            private void ActivePresentationChanged(OPowerPoint.Presentation presentation, DocumentWindow window)
            {
                Trigger();
            }

            /// <summary>
            /// This method gets the PowerPoint instance and is subscribing to the EApplication_WindowActivateEventHandle.
            /// </summary>
            protected override void OnEnabling(EnablingEventArgs e)
            {
                // When enabled, try and get the PowerPoint instance
                try
                {
                    powerPoint = (OPowerPoint.Application)Marshal.GetActiveObject("PowerPoint.Application");
                    powerPoint.WindowActivate += activePresentationChangedEvent;
                }
                catch (Exception ex)
                {
                    ErrorLog.AddError(ErrorType.Failure, Strings.PowerPoint_ApplicationNotFound);
                    Logger.Write(ex);
                    e.Cancel = true;
                }
            }

            /// <summary>
            /// This method is unsubscribing from the EApplication_WindowActivateEventHandle.
            /// </summary>
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
