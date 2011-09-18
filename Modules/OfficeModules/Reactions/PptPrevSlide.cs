using System;
using System.Runtime.InteropServices;
using MayhemCore;
using OfficeModules.Resources;
using OPowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace OfficeModules.Reactions
{
    [MayhemModule("PowerPoint: Last Slide", "Navigates to the previous slide")]
    public class PptPrevSlide : ReactionBase
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
                ErrorLog.AddError(ErrorType.Failure, Strings.PowerPoint_ApplicationNotFound);
                Logger.Write(e);
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
                    oApp.SlideShowWindows[1].View.Previous();
                }
                else if (windows == 0)
                {
                    ErrorLog.AddError(ErrorType.Warning, Strings.PowerPoint_NoWindowCantChange);
                }
                else // more than one window
                {
                    ErrorLog.AddError(ErrorType.Message, Strings.PowerPoint_MoreThanOneWindow);
                }
            }
            catch (Exception e)
            {
                ErrorLog.AddError(ErrorType.Warning, Strings.PowerPoint_CantChangeSlidesPrevious);
                Logger.Write(e);
            }
        }
    }
}
