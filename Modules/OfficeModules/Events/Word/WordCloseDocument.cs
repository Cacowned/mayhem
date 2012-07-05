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

        /// <summary>
        /// This method is called after the event is loaded.
        /// </summary>
        protected override void OnAfterLoad()
        {
            // Create the event handler delegate to attach
            closeDocumentEvent = DocumentClosed;
        }

        /// <summary>
        /// This method is called when the ApplicationEvents4_DocumentBeforeCloseEventHandler is triggered and will trigger this event.
        /// </summary> 
        /// <param name="document">The object representation of the current document</param>
        /// <param name="cancel">The closing action is stopped if the value is setted to 'true'</param>     
        public void DocumentClosed(OWord.Document document, ref bool cancel)
        {
            Trigger();
        }

        /// <summary>
        /// This method gets the Word instance and is subscribing to the ApplicationEvents4_DocumentBeforeCloseEventHandler.
        /// </summary>
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

        /// <summary>
        /// This method is unsubscribing from the ApplicationEvents4_DocumentBeforeCloseEventHandler.
        /// </summary>
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
