using System;
using System.Runtime.InteropServices;
using MayhemCore;
using OfficeModules.Resources;
using OExcel = Microsoft.Office.Interop.Excel;

namespace OfficeModules.Events.Excel
{
    /// <summary>
    /// An event that will be triggered when a workbook is opened.
    /// </summary>
    [MayhemModule("Excel: Open Workbook", "Triggers when a workbook is opened")]
    public class ExcelOpenWorkbook : EventBase
    {
        OExcel.Application excel;
        OExcel.AppEvents_WorkbookOpenEventHandler openWorkbookEvent;

        /// <summary>
        /// This method is called after the event is loaded.
        /// </summary>
        protected override void OnAfterLoad()
        {
            // Create the event handler delegate to attach
            openWorkbookEvent = WorkbookOpened;
        }

        /// <summary>
        /// This method is called when the AppEvents_WorkbookOpenEventHandler is triggered and will trigger this event.
        /// </summary>
        /// <param name="workbook">The object representation of the current workbook</param>
        private void WorkbookOpened(OExcel.Workbook workbook)
        {
            Trigger();
        }

        /// <summary>
        /// This method gets the Excel instance and is subscribing to the AppEvents_WorkbookOpenEventHandler.
        /// </summary>
        protected override void OnEnabling(EnablingEventArgs e)
        {
            // When enabled, try and get the Excel instance
            try
            {
                excel = (OExcel.Application)Marshal.GetActiveObject("Excel.Application");
                excel.WorkbookOpen += openWorkbookEvent;
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.Excel_ApplicationNotFound);
                Logger.Write(ex);
                e.Cancel = true;
            }
        }

        /// <summary>
        /// This method is unsubscribing from the AppEvents_WorkbookOpenEventHandler.
        /// </summary>
        protected override void OnDisabled(DisabledEventArgs e)
        {
            if (excel != null)
            {
                excel.WorkbookOpen -= openWorkbookEvent;
                excel = null;
            }
        }
    }
}
