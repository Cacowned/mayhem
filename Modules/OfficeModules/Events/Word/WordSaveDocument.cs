﻿using System;
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

        protected override void OnAfterLoad()
        {
            saveDocumentEvent = DocumentSaved;
        }

        public void DocumentSaved(OWord.Document document, ref bool saveAsUI, ref bool cancel)
        {
            Trigger();
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
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
