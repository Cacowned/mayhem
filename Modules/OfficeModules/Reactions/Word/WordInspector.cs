using System;
using System.Runtime.InteropServices;
using MayhemCore;
using OfficeModules.Resources;
using OWord = Microsoft.Office.Interop.Word;

namespace OfficeModules.Reactions.Word
{
    /// <summary>
    /// A reaction that will activate the inspector feature for the active document.
    /// </summary>
    [MayhemModule("Word: Activate Inspector", "Activates the inspector feature for a document")]
    public class WordInspector : ReactionBase
    {
        private OWord.Application app;

        /// <summary>
        /// If an instance of the Word application exits this method will activate the inspector feature for the active document.
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
                    dlg = app.Dialogs[OWord.WdWordDialog.wdDialogDocumentInspector];
                    dlg.Show(ref timeOut);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.Word_CantActivateInspector);
                Logger.Write(ex);
            }
        }
    }
}
