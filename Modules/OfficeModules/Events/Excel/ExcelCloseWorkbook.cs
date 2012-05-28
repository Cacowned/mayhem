using System;
using System.Runtime.InteropServices;
using MayhemCore;
using OfficeModules.Resources;
using OExcel = Microsoft.Office.Interop.Excel;

namespace OfficeModules.Events.Excel
{
    [MayhemModule("Excel: Close Workbook", "Triggers when an workbook is closed")]
    public class ExcelCloseWorkbook : EventBase
    {
        OExcel.Application excel;
        OExcel.AppEvents_WorkbookBeforeCloseEventHandler closeWorkbookEvent;

        protected override void OnAfterLoad()
        {
            // Create the event handler delegate to attach
            closeWorkbookEvent = WorkbookClosed;
        }

        private void WorkbookClosed(OExcel.Workbook workbook, ref bool cancel)
        {
            Trigger();
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
            // When enabled, try and get the PowerPoint instance
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
