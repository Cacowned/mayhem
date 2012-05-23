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
    [DataContract]
    [MayhemModule("PowerPoint: Export notes", "Exports the notes from the active presentation")]
    public class PptExportNotes : ReactionBase, IWpfConfigurable
    {
        private OPowerPoint.Application app;
        private StreamWriter streamWriter = null;

        /// <summary>
        /// The path of the file were the notes would be saved
        /// </summary>
        [DataMember]
        private string FileName
        {
            get;
            set;
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var pptExportConfig = configurationControl as PowerPointExportConfig;
            FileName = pptExportConfig.Filename;
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
            if (!File.Exists(FileName))
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.General_FileNotFound);
                e.Cancel = true;
            }
        }

        public override void Perform()
        {
            if (File.Exists(FileName))
            {
                try
                {
                    //if the file exists we try to open it
                    FileStream stream = File.Open(FileName, FileMode.Create);
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
                        //the current presentation doesn't have notes
                        ErrorLog.AddError(ErrorType.Message, Strings.PowerPoint_NoNotes);
                    }
                    else
                    {
                        foreach (OPowerPoint.Slide slide in activePresentation.Slides)
                        {
                            if (slide.HasNotesPage == Microsoft.Office.Core.MsoTriState.msoTrue)
                            {
                                if (slide.NotesPage.Shapes.Placeholders.Count >= 2) //if we have notes(the notes are kept on the position 2 in the placeholders collection) we save them in the selected file
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
            }
            finally
            {
                app = null;
            }
        }

        #region IWpfConfigurable Methods

        public WpfConfiguration ConfigurationControl
        {
            get { return new PowerPointExportConfig(FileName); }
        }

        #endregion

        #region IConfigurable Members
      
        public string GetConfigString()
        {
            return string.Format(CultureInfo.CurrentCulture, Strings.PowerPoint_ExportConfigString, Path.GetFileName(FileName));
        }

        #endregion
    }
}
