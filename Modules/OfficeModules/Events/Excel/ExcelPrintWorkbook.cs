using System;
using System.Runtime.InteropServices;
using MayhemCore;
using OfficeModules.Resources;
using OExcel = Microsoft.Office.Interop.Excel;

namespace OfficeModules.Events.Excel
{
    /// <summary>
    /// An event that will be triggered when a workbook is printed.
    /// </summary>
    [MayhemModule("Excel: Print Workbook", "Triggers when a workbook is printed")]
    public class ExcelPrintWorkbook : EventBase
    {
        OExcel.Application excel;
        OExcel.AppEvents_WorkbookBeforePrintEventHandler printWorkbookEvent;

        /// <summary>
        /// This method is called after the event is loaded.
        /// </summary>
        protected override void OnAfterLoad()
        {
            // Create the event handler delegate to attach
            printWorkbookEvent = WorkbookPrinted;
        }

        /// <summary>
        /// This method is called when the AppEvents_WorkbookBeforePrintEventHandler is triggered and will trigger this event.
        /// </summary>
        /// <param name="workbook">The object representation of the current workbook</param>
        /// <param name="cancel">The printing action is stopped if the value is setted to 'true'</param>
        private void WorkbookPrinted(OExcel.Workbook workbook, ref bool cancel)
        {
            Trigger();
        }

        /// <summary>
        /// This method gets the Excel instance and is subscribing to the AppEvents_WorkbookBeforePrintEventHandler.
        /// </summary>
        protected override void OnEnabling(EnablingEventArgs e)
        {
            // When enabled, try and get the Excel instance
            try
            {
                excel = (OExcel.Application)Marshal.GetActiveObject("Excel.Application");
                excel.WorkbookBeforePrint += printWorkbookEvent;
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.Excel_ApplicationNotFound);
                Logger.Write(ex);
                e.Cancel = true;
            }
        }

        /// <summary>
        /// This method is unsubscribing from the AppEvents_WorkbookBeforePrintEventHandler.
        /// </summary>
        protected override void OnDisabled(DisabledEventArgs e)
        {
            if (excel != null)
            {
                excel.WorkbookBeforePrint -= printWorkbookEvent;
                excel = null;
            }
        }
    }
}
