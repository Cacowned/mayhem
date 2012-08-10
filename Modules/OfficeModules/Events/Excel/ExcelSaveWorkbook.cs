using System;
using System.Runtime.InteropServices;
using MayhemCore;
using OfficeModules.Resources;
using OExcel = Microsoft.Office.Interop.Excel;

namespace OfficeModules.Events.Excel
{
    /// <summary>
    /// An event that will be triggered when a workbook is saved.
    /// </summary>
    [MayhemModule("Excel: Save Workbook", "Triggers when a workbook is saved")]
    public class ExcelSaveWorkbook : EventBase
    {
        OExcel.Application excel;
        OExcel.AppEvents_WorkbookAfterSaveEventHandler saveWorkbookEvent;

        protected override void OnAfterLoad()
        {
            saveWorkbookEvent = WorkbookSaved;
        }

        private void WorkbookSaved(OExcel.Workbook workbook, bool success)
        {
            if (success) // Only if the save action was succesful the event will be triggered
                Trigger();
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
            try
            {
                excel = (OExcel.Application)Marshal.GetActiveObject("Excel.Application");
                excel.WorkbookAfterSave += saveWorkbookEvent;
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
                excel.WorkbookAfterSave -= saveWorkbookEvent;
                excel = null;
            }
        }
    }
}
