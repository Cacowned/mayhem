using System;
using System.Runtime.InteropServices;
using MayhemCore;
using OfficeModules.Resources;
using OWord = Microsoft.Office.Interop.Word;

namespace OfficeModules.Events.Word
{
    /// <summary>
    /// An event that will be triggered when a document is saved.
    /// </summary>
    [MayhemModule("Word: Save Document", "Triggers when a document is saved")]
    public class WordSaveDocument : EventBase
    {
        OWord.Application word;
        OWord.ApplicationEvents4_DocumentBeforeSaveEventHandler saveDocumentEvent;

        /// <summary>
        /// This method is called after the event is loaded.
        /// </summary>
        protected override void OnAfterLoad()
        {
            // Create the event handler delegate to attach
            saveDocumentEvent = DocumentSaved;
        }

        /// <summary>
        /// This method is called when the ApplicationEvents4_DocumentBeforeSaveEventHandler is triggered and will trigger this event.
        /// </summary>
        /// <param name="document">The object representation of the current document</param>
        /// <param name="saveAsUI">True if the Save As dialog box will be displayed, false otherwise</param>
        /// <param name="cancel">The saving action is stopped if the value is setted to 'true'</param>
        public void DocumentSaved(OWord.Document document, ref bool saveAsUI, ref bool cancel)
        {
            Trigger();
        }

        /// <summary>
        /// This method gets the Word instance and is subscribing to the ApplicationEvents4_DocumentBeforeSaveEventHandler.
        /// </summary>
        protected override void OnEnabling(EnablingEventArgs e)
        {
            // When enabled, try and get the Word instance
            try
            {
                word = (OWord.Application)Marshal.GetActiveObject("Word.Application");
                word.DocumentBeforeSave += saveDocumentEvent;
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.Word_ApplicationNotFound);
                Logger.Write(ex);
                e.Cancel = true;
            }
        }

        /// <summary>
        /// This method is unsubscribing from the ApplicationEvents4_DocumentBeforeSaveEventHandler.
        /// </summary>
        protected override void OnDisabled(DisabledEventArgs e)
        {
            if (word != null)
            {
                word.DocumentBeforeSave -= saveDocumentEvent;
                word = null;
            }
        }
    }
}
