using System;
using System.Runtime.InteropServices;
using MayhemCore;
using OfficeModules.Resources;
using OPowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace OfficeModules.Reactions
{
    [MayhemModule("PowerPoint: View Slideshow", "Opens the slideshow window for the active presentation")]
    public class PptViewSlideShow : ReactionBase
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
                int presentations = app.Presentations.Count;

                if (presentations == 0)
                {
                    ErrorLog.AddError(ErrorType.Warning, Strings.PowerPoint_NoActivePresentation);
                }
                else
                {                    
                    app.ActivePresentation.SlideShowSettings.Run();
                    app.ActivePresentation.SlideShowWindow.Activate();
                }
            }
            catch (Exception e)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.PowerPoint_CantStartSlideShow);
                Logger.Write(e);
            }
            finally
            {
                app = null;
            }
        }
    }
}