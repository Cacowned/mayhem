using System;
using System.Runtime.InteropServices;
using MayhemCore;
using OfficeModules.Resources;
using OExcel = Microsoft.Office.Interop.Excel;

namespace OfficeModules.Events.Excel
{
    [MayhemModule("Excel: Open Workbook", "Triggers when a workbook is opened")]
    public class ExcelOpenWorkbook : EventBase
    {
        OExcel.Application excel;
        OExcel.AppEvents_WorkbookOpenEventHandler openWorkbookEvent;

        protected override void OnAfterLoad()
        {
            // Create the event handler delegate to attach
            openWorkbookEvent = WorkbookOpened;
        }

        private void WorkbookOpened(OExcel.Workbook workbook)
        {
            Trigger();
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
            // When enabled, try and get the PowerPoint instance
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
