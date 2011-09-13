using System;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using MayhemCore;
using OPowerPoint = Microsoft.Office.Interop.PowerPoint;
using System.Diagnostics;

namespace OfficeModules.Reactions.PowerPoint
{
    [DataContract]
    [MayhemModule("PowerPoint: Next Slide", "Navigates to the next slide")]
    public class PptNextSlide : ReactionBase
    {
        private OPowerPoint.Application oApp;

        public override void Enable()
        {
            try
            {
                oApp = (OPowerPoint.Application)Marshal.GetActiveObject("PowerPoint.Application");
                base.Enable();
            }
            catch (Exception e)
            {
                ErrorLog.AddError(ErrorType.Failure, "Unable to find the PowerPoint application window.");
                Debug.Write(e);
            }
        }

        public override void Disable()
        {
            base.Disable();

            oApp = null;
        }

        public override void Perform()
        {
            try
            {
                int windows = oApp.SlideShowWindows.Count;
                // If we have a presentation window, go to the next slide
                if (windows == 1)
                {
                    oApp.SlideShowWindows[1].View.Next();
                }
                else if (windows == 0)
                {
                    ErrorLog.AddError(ErrorType.Warning, "You must be in a slideshow view to change slide");
                }
                else // more than one window
                {
                    ErrorLog.AddError(ErrorType.Message, "More than one slideshow window detected, using the first one.");
                }
            }
            catch (Exception e)
            {
                ErrorLog.AddError(ErrorType.Warning, "Can't go to the next slide.");
                Debug.Write(e);
            }
        }
    }
}
