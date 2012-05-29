using System;
using System.Runtime.InteropServices;
using MayhemCore;
using OfficeModules.Resources;
using OWord = Microsoft.Office.Interop.Word;

namespace OfficeModules.Events
{
    [MayhemModule("Word: Close Document", "Triggers when a document is closed")]
    public class WordCloseDocument : EventBase
    {
        OWord.Application word;
        OWord.ApplicationEvents4_DocumentBeforeCloseEventHandler closeDocumentEvent;

        protected override void OnAfterLoad()
        {
            // Create the event handler delegate to attach
            closeDocumentEvent = DocumentClosed;
        }

        public void DocumentClosed(OWord.Document document, ref bool cancel)
        {
            Trigger();
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
            // When enabled, try and get the Word instance
            try
            {
                word = (OWord.Application)Marshal.GetActiveObject("Word.Application");
                word.DocumentBeforeClose += closeDocumentEvent;
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
                word.DocumentBeforeClose -= closeDocumentEvent;
                word = null;
            }
        }
    }
}
