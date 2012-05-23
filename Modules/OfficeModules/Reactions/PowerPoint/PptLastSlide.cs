using System;
using System.Runtime.InteropServices;
using MayhemCore;
using OfficeModules.Resources;
using OPowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace OfficeModules.Reactions
{
    [MayhemModule("PowerPoint: Last Slide", "Navigates to the last slide")]
    public class PptLastSlide : ReactionBase
    {
        private OPowerPoint.Application app;

        public override void Perform()
        {
            try
            {
                app = (OPowerPoint.Application)Marshal.GetActiveObject("PowerPoint.Application");
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.PowerPoint_ApplicationNotFound);
                Logger.Write(ex);

                return;
            }

            try
            {
                int windows = app.SlideShowWindows.Count;

                if (windows == 0)
                {
                    ErrorLog.AddError(ErrorType.Warning, Strings.PowerPoint_NoWindowCantChange);
                }
                else
                {
                    if (windows > 1)
                    {
                        // we've got more than one
                        ErrorLog.AddError(ErrorType.Message, Strings.PowerPoint_MoreThanOneWindow);
                    }

                    app.SlideShowWindows[1].View.Last();
                }
            }
            catch (Exception e)
            {
                ErrorLog.AddError(ErrorType.Warning, Strings.PowerPoint_CantChangeSlidesLast);
                Logger.Write(e);
            }
            finally
            {
                app = null;
            }
        }
    }
}
