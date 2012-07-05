using System;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using OfficeModules.Resources;
using OfficeModules.Wpf;
using OPowerPoint = Microsoft.Office.Interop.PowerPoint;

namespace OfficeModules.Reactions.PowerPoint
{
    /// <summary>
    /// A reaction that will export the text of the active presentation.
    /// </summary>
    [DataContract]
    [MayhemModule("PowerPoint: Export Text", "Exports the text of the slides from the active presentation")]
    public class PptExportText : ReactionBase, IWpfConfigurable
    {
        /// <summary>
        /// The path of the file were the notes would be saved
        /// </summary>
        [DataMember]
        private string fileName;

        private OPowerPoint.Application app;
        private StreamWriter streamWriter = null;

        /// <summary>
        /// This method checks if the selected file exists.
        /// </summary>
        protected override void OnEnabling(EnablingEventArgs e)
        {
            if (!File.Exists(fileName))
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.General_FileNotFound);
                e.Cancel = true;
            }
        }

        /// <summary>
        /// This method will get the instance of the PowerPoint application and will export the text of the active presentation to the selected file.
        /// </summary>
        public override void Perform()
        {
            if (File.Exists(fileName))
            {
                streamWriter = null;

                try
                {
                    FileStream stream = File.Open(fileName, FileMode.Create);
                    streamWriter = new StreamWriter(stream);
                }
                catch (Exception ex)
                {
                    ErrorLog.AddError(ErrorType.Failure, Strings.General_CantOpenFile);
                    Logger.WriteLine(ex);

                    return;
                }
            }
            else
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.General_FileNotFound);
                return;
            }

            try
            {
                app = (OPowerPoint.Application)Marshal.GetActiveObject("PowerPoint.Application");
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.PowerPoint_ApplicationNotFound);
                Logger.Write(ex);

                try
                {
                    if (streamWriter != null)
                        streamWriter.Close();
                }
                catch (IOException e)
                {
                    ErrorLog.AddError(ErrorType.Failure, Strings.CantCloseFileStream);
                    Logger.WriteLine(e);
                }

                return;
            }

            try
            {
                OPowerPoint.Presentation activePresentation = app.ActivePresentation;

                if (activePresentation == null)
                {
                    ErrorLog.AddError(ErrorType.Failure, Strings.PowerPoint_NoActivePresentation);
                }
                else
                {
                    foreach (OPowerPoint.Slide slide in activePresentation.Slides)
                    {
                        foreach (OPowerPoint.Shape shape in slide.Shapes)
                        {
                            if (shape.HasTextFrame == Microsoft.Office.Core.MsoTriState.msoTrue)
                                if (shape.TextFrame.HasText == Microsoft.Office.Core.MsoTriState.msoTrue)
                                    streamWriter.WriteLine(shape.TextFrame.TextRange.Text);
                        }
                    }
                }

                streamWriter.Close();
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.PowerPoint_CantExportText);
                Logger.Write(ex);

                try
                {
                    if (streamWriter != null)
                        streamWriter.Close();
                }
                catch (IOException e)
                {
                    ErrorLog.AddError(ErrorType.Failure, Strings.CantCloseFileStream);
                    Logger.WriteLine(e);
                }
            }
            finally
            {
                app = null;
            }
        }

        #region IWpfConfigurable Methods

        public WpfConfiguration ConfigurationControl
        {
            get { return new PowerPointExportConfig(fileName,Strings.PptExportText_Title); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var pptExportConfig = configurationControl as PowerPointExportConfig;

            fileName = pptExportConfig.FileName;
        }

        #endregion

        #region IConfigurable Members

        public string GetConfigString()
        {
            return string.Format(CultureInfo.CurrentCulture, Strings.PowerPoint_ExportConfigString, Path.GetFileName(fileName));
        }

        #endregion
    }
}
