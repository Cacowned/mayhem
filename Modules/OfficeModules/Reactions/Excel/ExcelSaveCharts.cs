﻿using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using OfficeModules.Resources;
using OfficeModules.Wpf;
using OExcel = Microsoft.Office.Interop.Excel;

namespace OfficeModules.Reactions.Excel
{
    [DataContract]
    [MayhemModule("Excel: Save charts", "Saves the charts from the current workbook")]
    public class ExcelSaveCharts : ReactionBase, IWpfConfigurable
    {
        OExcel.Application app;

        [DataMember]
        private string fileName;

        protected override void OnEnabling(EnablingEventArgs e)
        {
            if (!Directory.Exists(fileName))
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.General_DirectoryNotFound);
                e.Cancel = true;
            }
        }

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
                    int count = 1;
                    foreach (OExcel.Worksheet sheet in workbook.Sheets)
                        foreach (OExcel.ChartObject obj in sheet.ChartObjects(Type.Missing))
                        {
                            obj.Chart.Export(fileName + "\\chart" + count + ".jpg");
                            count++;
                        }
                }
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.Excel_CantSaveCharts);
                Logger.Write(ex);
            }
        }

        #region IWpfConfigurable Methods

        public WpfConfiguration ConfigurationControl
        {
            get { return new ExcelSaveChartsConfig(fileName); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var excelSaveChartsConfig = configurationControl as ExcelSaveChartsConfig;
        
            fileName = excelSaveChartsConfig.Filename;
        }

        #endregion

        #region IConfigurable Members

        public string GetConfigString()
        {
            return string.Format(CultureInfo.CurrentCulture, Strings.General_DirectorySaveConfigString, Path.GetFullPath(fileName));
        }

        #endregion
    }
}
