﻿using System;
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

            protected override void OnAfterLoad()
            {
                activePresentationChangedEvent = ActivePresentationChanged;
            }

            private void ActivePresentationChanged(OPowerPoint.Presentation presentation, DocumentWindow window)
            {
                Trigger();
            }

            protected override void OnEnabling(EnablingEventArgs e)
            {
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
