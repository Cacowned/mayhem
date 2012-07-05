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

        /// <summary>
        /// This method is called after the event is loaded.
        /// </summary>
        protected override void OnAfterLoad()
        {
            // Create the event handler delegate to attach
            saveWorkbookEvent = WorkbookSaved;
        }

        /// <summary>
        /// This method is called when the AppEvents_WorkbookAfterSaveEventHandler is triggered and will trigger this event.
        /// </summary>
        /// <param name="workbook">The object representation of the current workbook</param>
        /// <param name="success">The value will be 'true' if the workbook was successfully saved, 'false' otherwise</param>
        private void WorkbookSaved(OExcel.Workbook workbook, bool success)
        {
            if (success) // Only if the save action was succesful the event will be triggered
                Trigger();
        }

        /// <summary>
        /// This method gets the Excel instance and is subscribing to the AppEvents_WorkbookAfterSaveEventHandler.
        /// </summary>
        protected override void OnEnabling(EnablingEventArgs e)
        {
            // When enabled, try and get the Excel instance
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

        /// <summary>
        /// This method is unsubscribing from the AppEvents_WorkbookAfterSaveEventHandler.
        /// </summary>
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
