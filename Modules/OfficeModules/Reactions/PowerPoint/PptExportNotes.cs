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
    /// A reaction that will export the notes of the active presentation.
    /// </summary>
    [DataContract]
    [MayhemModule("PowerPoint: Export Notes", "Exports the notes from the active presentation")]
    public class PptExportNotes : ReactionBase, IWpfConfigurable
    {
        /// <summary>
        /// The path of the file were the notes would be saved.
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
        /// This method will get the instance of the PowerPoint application and will export the notes of the active presentation to the selected file.
        /// </summary>
        public override void Perform()
        {
            streamWriter = null;

            if (File.Exists(fileName))
            {
                try
                {
                    // If the file exists we try to open it
                    FileStream stream = File.Open(fileName, FileMode.Create);
                    streamWriter = new StreamWriter(stream);
                }
                catch (Exception ex)
                {
                    ErrorLog.AddError(ErrorType.Failure, Strings.General_CantOpenFile);
                    Logger.WriteLine(ex.Message);
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
                Logger.WriteLine(ex);

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
                    if (!activePresentation.HasNotesMaster)
                    {
                        // The current presentation doesn't have notes
                        ErrorLog.AddError(ErrorType.Message, Strings.PowerPoint_NoNotes);
                    }
                    else
                    {
                        foreach (OPowerPoint.Slide slide in activePresentation.Slides)
                        {
                            if (slide.HasNotesPage == Microsoft.Office.Core.MsoTriState.msoTrue)
                            {
                                // If we have notes(the notes are kept on the position 2 in the placeholders collection) we save their text in the selected file
                                if (slide.NotesPage.Shapes.Placeholders.Count >= 2)
                                {
                                    streamWriter.WriteLine(slide.NotesPage.Shapes.Placeholders[2].TextFrame.TextRange.Text);
                                }
                            }
                        }
                    }

                streamWriter.Close();
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Warning, Strings.PowerPoint_CantExportNotes);
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
            get { return new PowerPointExportConfig(fileName, Strings.PptExportNotes_Title); }
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
