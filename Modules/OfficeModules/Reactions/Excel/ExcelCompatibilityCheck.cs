﻿using System;
using System.Runtime.InteropServices;
using MayhemCore;
using OfficeModules.Resources;
using OExcel = Microsoft.Office.Interop.Excel;

namespace OfficeModules.Reactions.Excel
{
    [MayhemModule("Excel: Check Compatibility", "Checks the compatibility of the current workbook")]
    public class ExcelCompatibilityCheck : ReactionBase
    {
        OExcel.Application app;
        public override void Perform()
        {
            try
            {
                app = (OExcel.Application)Marshal.GetActiveObject("Excel.Application");
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.Excel_ApplicationNotFound);
                Logger.Write(ex);

                return;
            }

            try
            {
                OExcel.Workbook workbook = app.ActiveWorkbook;

                if (workbook == null)
                {
                    ErrorLog.AddError(ErrorType.Failure, Strings.Excel_NoActiveWorkbook);
                }
                else
                {
                    // When the workbook is saved, if it's not compatible, the compatibility window will be displayed
                    workbook.CheckCompatibility = true;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.Excel_CantCheckCompatibility);
                Logger.Write(ex);
            }
        }
    }
}