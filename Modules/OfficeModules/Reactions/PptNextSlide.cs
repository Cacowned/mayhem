using System;
using System.Runtime.InteropServices;
using MayhemCore;
using OfficeModules.Resources;
using OPowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace OfficeModules.Reactions
{
    [MayhemModule("PowerPoint: Next Slide", "Navigates to the next slide")]
    public class PptNextSlide : ReactionBase
    {
        private OPowerPoint.Application app;

        protected override void OnEnabling(EnablingEventArgs e)
        {
            try
            {
                app = (OPowerPoint.Application)Marshal.GetActiveObject("PowerPoint.Application");
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
            app = null;
        }

        public override void Perform()
        {
            try
            {
                int windows = app.SlideShowWindows.Count;

                // If we have a presentation window, go to the next slide
                if (windows == 1)
                {
                    app.SlideShowWindows[1].View.Next();
                }
                else if (windows == 0)
                {
                    ErrorLog.AddError(ErrorType.Warning, Strings.PowerPoint_NoWindowCantChange);
                }
                else
                {
                    // more than one window
                    ErrorLog.AddError(ErrorType.Message, Strings.PowerPoint_MoreThanOneWindow);
                }
            }
            catch (Exception e)
            {
                ErrorLog.AddError(ErrorType.Warning, Strings.PowerPoint_CantChangeSlidesNext);
                Logger.Write(e);
            }
        }
    }
}
