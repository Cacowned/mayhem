using System;
using System.Runtime.InteropServices;
using MayhemCore;
using OfficeModules.Resources;
using OWord = Microsoft.Office.Interop.Word;

namespace OfficeModules.Events
{
    /// <summary>
    /// An event that will be triggered when a document is closed.
    /// </summary>
    [MayhemModule("Word: Close Document", "Triggers when a document is closed")]
    public class WordCloseDocument : EventBase
    {
        OWord.Application word;
        OWord.ApplicationEvents4_DocumentBeforeCloseEventHandler closeDocumentEvent;

        protected override void OnAfterLoad()
        {
            closeDocumentEvent = DocumentClosed;
        }

        public void DocumentClosed(OWord.Document document, ref bool cancel)
        {
            Trigger();
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
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
