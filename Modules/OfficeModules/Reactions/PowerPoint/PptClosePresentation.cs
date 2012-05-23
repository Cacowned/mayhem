using System;
using System.Runtime.InteropServices;
using MayhemCore;
using OfficeModules.Resources;
using OPowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace OfficeModules.Reactions
{
    [MayhemModule("PowerPoint: Close Presentation", "Closes the active presentation")]
    public class PptClosePresentation : ReactionBase
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
                    ErrorLog.AddError(ErrorType.Failure, Strings.PowerPoint_NoActivePresentation);
                }
                else
                {
                    app.ActivePresentation.Close();
                }
            }
            catch (Exception e)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.PowerPoint_CantClosePresentation);
                Logger.Write(e);
            }
            finally
            {
                app = null;
            }
        }
    }
}
