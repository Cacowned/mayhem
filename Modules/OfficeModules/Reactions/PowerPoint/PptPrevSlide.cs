﻿using System;
using System.Runtime.InteropServices;
using MayhemCore;
using OfficeModules.Resources;
using OPowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace OfficeModules.Reactions
{
    /// <summary>
    /// A reaction that will navigate to the previous slide of the active slideshow.
    /// </summary>
    [MayhemModule("PowerPoint: Previous Slide", "Navigates to the previous slide")]
    public class PptPrevSlide : ReactionBase
    {
        private OPowerPoint.Application app;

        /// <summary>
        /// If an instance of the PowerPoint application exits this method will navigate to the previous slide of the active slideshow.
        /// </summary>
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
                    ErrorLog.AddError(ErrorType.Failure, Strings.PowerPoint_NoWindowCantChange);
                }
                else
                {
                    if (windows > 1)
                    {
                        // We've got more than one
                        ErrorLog.AddError(ErrorType.Message, Strings.PowerPoint_MoreThanOneWindow);
                    }

                    app.SlideShowWindows[1].View.Previous();
                }
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.PowerPoint_CantChangeSlidesPrevious);
                Logger.Write(ex);
            }
            finally
            {
                app = null;
            }
        }
    }
}
