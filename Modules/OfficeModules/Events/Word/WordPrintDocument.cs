using System;
using System.Runtime.InteropServices;
using MayhemCore;
using OfficeModules.Resources;
using OWord = Microsoft.Office.Interop.Word;

namespace OfficeModules.Events.Word
{
    [MayhemModule("Word: Print Document", "Triggers when a document is printed")]
    public class WordPrintDocument : EventBase
    {
        OWord.Application word;
        OWord.ApplicationEvents4_DocumentBeforePrintEventHandler printDocumentEvent;

        protected override void OnAfterLoad()
        {
            // Create the event handler delegate to attach
            printDocumentEvent = DocumentPrinted;
        }

        public void DocumentPrinted(OWord.Document document, ref bool cancel)
        {
            Trigger();
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
            // When enabled, try and get the Word instance
            try
            {
                word = (OWord.Application)Marshal.GetActiveObject("Word.Application");
                word.DocumentBeforePrint += printDocumentEvent;
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.Word_ApplicationNotFound);
                Logger.Write(ex);
                e.Cancel = true;
            }
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            if (word != null)
            {
                word.DocumentBeforePrint -= printDocumentEvent;
                word = null;
            }
        }
    }
}
