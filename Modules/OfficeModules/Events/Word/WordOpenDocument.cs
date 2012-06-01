﻿using System;
using System.Runtime.InteropServices;
using MayhemCore;
using OfficeModules.Resources;
using OWord = Microsoft.Office.Interop.Word;

namespace OfficeModules.Events
{
    [MayhemModule("Word: Open Document", "Triggers when a document is opened")]
    public class WordOpenDocument : EventBase
    {
        OWord.Application word;
        OWord.ApplicationEvents4_DocumentOpenEventHandler openDocumentEvent;

        protected override void OnAfterLoad()
        {             
            // Create the event handler delegate to attach
            openDocumentEvent = DocumentOpened;
        }

        private void DocumentOpened(OWord.Document document)
        {            
            Trigger();
        }

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
