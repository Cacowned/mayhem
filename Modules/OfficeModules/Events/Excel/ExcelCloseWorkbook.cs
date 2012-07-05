using System;
using System.Runtime.InteropServices;
using MayhemCore;
using OfficeModules.Resources;
using OExcel = Microsoft.Office.Interop.Excel;

namespace OfficeModules.Events.Excel
{
    /// <summary>
    /// An event that will be triggered when a workbook is closed.
    /// </summary>
    [MayhemModule("Excel: Close Workbook", "Triggers when a workbook is closed")]
    public class ExcelCloseWorkbook : EventBase
    {
        OExcel.Application excel;
        OExcel.AppEvents_WorkbookBeforeCloseEventHandler closeWorkbookEvent;

        /// <summary>
        /// This method is called after the event is loaded.
        /// </summary>
        protected override void OnAfterLoad()
        {
            // Create the event handler delegate to attach
            closeWorkbookEvent = WorkbookClosed;
        }

        /// <summary>
        /// This method is called when the AppEvents_WorkbookBeforeCloseEventHandler is triggered and will trigger this event.
        /// </summary>
        /// <param name="workbook">The object representation of the current workbook</param>
        /// <param name="cancel">The closing action is stopped if the value is setted to 'true'</param>
        private void WorkbookClosed(OExcel.Workbook workbook, ref bool cancel)
        {
            Trigger();
        }

        /// <summary>
        /// This method gets the Excel instance and is subscribing to the AppEvents_WorkbookBeforeCloseEventHandler.
        /// </summary>
        protected override void OnEnabling(EnablingEventArgs e)
        {
            // When enabled, try and get the Excel instance
            try
            {
                excel = (OExcel.Application)Marshal.GetActiveObject("Excel.Application");
                excel.WorkbookBeforeClose += closeWorkbookEvent;
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.Excel_ApplicationNotFound);
                Logger.Write(ex);
                e.Cancel = true;
            }
        }

        /// <summary>
        /// This method is unsubscribing from the AppEvents_WorkbookBeforeCloseEventHandler.
        /// </summary>
        protected override void OnDisabled(DisabledEventArgs e)
        {
            if (excel != null)
            {
                excel.WorkbookBeforeClose -= closeWorkbookEvent;
                excel = null;
            }
        }
    }
}
