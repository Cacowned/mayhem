using System;
using System.Runtime.InteropServices;
using MayhemCore;
using OfficeModules.Resources;
using OWord = Microsoft.Office.Interop.Word;

namespace OfficeModules.Reactions.Word
{
    [MayhemModule("Word: Check Spelling", "Activates the spell checking for a document")]
    public class WordCheckSpelling : ReactionBase
    {
        private OWord.Application app;

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
                    dlg = app.Dialogs[OWord.WdWordDialog.wdDialogToolsSpellingAndGrammar];
                    dlg.Show(ref timeOut);
                }
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.Word_CantActivateSpellChecking);
                Logger.Write(ex);
            }
        }
    }
}
