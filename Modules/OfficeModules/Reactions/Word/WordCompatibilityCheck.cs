using System;
using System.Runtime.InteropServices;
using MayhemCore;
using OfficeModules.Resources;
using OWord = Microsoft.Office.Interop.Word;

namespace OfficeModules.Reactions.Word
{
    /// <summary>
    /// A reaction that will activate the compatibility checking for the active document.
    /// </summary>
    [MayhemModule("Word: Check Compatibility", "Activates the compatibility checking for a document")]
    public class WordCompatibilityCheck : ReactionBase
    {
        private OWord.Application app;

        /// <summary>
        /// This method will get the instance of the Word application and will activate the compatibility checking for the active document.
        /// </summary>
        public override void Perform()
        {
            try
            {
                app = (OWord.Application)Marshal.GetActiveObject("Word.Application");
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.Word_ApplicationNotFound);
                Logger.Write(ex);

                return;
            }

            if (app == null)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.Word_ApplicationNotFound);
                return;
            }

            try
            {
                OWord.Document activeDocument = app.ActiveDocument;

                if (activeDocument == null)
                {
                    ErrorLog.AddError(ErrorType.Failure, Strings.Word_NoActiveDocument);
                }
                else
                {
                    OWord.Dialog dlg;
                    Object timeOut = Type.Missing;
                    dlg = app.Dialogs[OWord.WdWordDialog.wdDialogCompatibilityChecker];
                    dlg.Show(ref timeOut);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.Word_CantActivateCompatibilityCheck);
                Logger.Write(ex);
            }
        }
    }
}
