using System;
using System.Runtime.InteropServices;
using MayhemCore;
using OfficeModules.Resources;
using OExcel = Microsoft.Office.Interop.Excel;

namespace OfficeModules.Events.Excel
{
    [MayhemModule("Excel: Print Workbook", "Triggers when a workbook is printed")]
    public class ExcelPrintWorkbook : EventBase
    {
        OExcel.Application excel;
        OExcel.AppEvents_WorkbookBeforePrintEventHandler printWorkbookEvent;

        protected override void OnAfterLoad()
        {
            // Create the event handler delegate to attach
            printWorkbookEvent = WorkbookPrinted;
        }

        private void WorkbookPrinted(OExcel.Workbook workbook, ref bool cancel)
        {
            Trigger();
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
            // When enabled, try and get the PowerPoint instance
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
