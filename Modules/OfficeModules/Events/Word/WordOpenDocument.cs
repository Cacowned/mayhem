using System;
using System.Runtime.InteropServices;
using MayhemCore;
using OfficeModules.Resources;
using OWord = Microsoft.Office.Interop.Word;

namespace OfficeModules.Events
{
    /// <summary>
    /// An event that will be triggered when a document is opened.
    /// </summary>
    [MayhemModule("Word: Open Document", "Triggers when a document is opened")]
    public class WordOpenDocument : EventBase
    {
        OWord.Application word;
        OWord.ApplicationEvents4_DocumentOpenEventHandler openDocumentEvent;

        /// <summary>
        /// This method is called after the event is loaded.
        /// </summary>
        protected override void OnAfterLoad()
        {             
            // Create the event handler delegate to attach
            openDocumentEvent = DocumentOpened;
        }

        /// <summary>
        /// This method is called when the ApplicationEvents4_DocumentOpenEventHandler is triggered and will trigger this event.
        /// </summary> 
        /// <param name="document">The object representation of the current document</param>
        private void DocumentOpened(OWord.Document document)
        {            
            Trigger();
        }

        /// <summary>
        /// This method gets the Word instance and is subscribing to the ApplicationEvents4_DocumentOpenEventHandler.
        /// </summary>
        protected override void OnEnabling(EnablingEventArgs e)
        {
            // When enabled, try and get the Word instance
            try
            {
                word = (OWord.Application)Marshal.GetActiveObject("Word.Application");
                word.DocumentOpen += openDocumentEvent;
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.Word_ApplicationNotFound);                
                Logger.Write(ex);
                e.Cancel = true;
            }
        }

        /// <summary>
        /// This method is unsubscribing from the ApplicationEvents4_DocumentOpenEventHandler.
        /// </summary>
        protected override void OnDisabled(DisabledEventArgs e)
        {
            if (word != null)
            {
                word.DocumentOpen -= openDocumentEvent;
                word = null;
            }
        }
    }
}
