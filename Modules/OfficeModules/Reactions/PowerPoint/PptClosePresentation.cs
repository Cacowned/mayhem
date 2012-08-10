using System;
using System.Runtime.InteropServices;
using MayhemCore;
using OfficeModules.Resources;
using OPowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace OfficeModules.Reactions
{
    /// <summary>
    /// A reaction that will close the active PowerPoint presentation.
    /// </summary>
    [MayhemModule("PowerPoint: Close Presentation", "Closes the active presentation")]
    public class PptClosePresentation : ReactionBase
    {
        private OPowerPoint.Application app;

        /// <summary>
        /// If an instance of the PowerPoint application exits this method will close the active presentation.
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
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.PowerPoint_CantClosePresentation);
                Logger.Write(ex);
            }
            finally
            {
                app = null;
            }
        }
    }
}
