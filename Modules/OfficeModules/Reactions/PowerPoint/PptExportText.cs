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
    [MayhemModule("PowerPoint: Export Text", "Exports the text of the slides from the active presentation")]
    public class PptExportText : ReactionBase, IWpfConfigurable
    {
        private OPowerPoint.Application app;
        private StreamWriter streamWriter = null;

        /// <summary>
        /// The path of the file were the notes would be saved
        /// </summary>
        [DataMember]
        private string fileName;	

		public void OnSaved(WpfConfiguration configurationControl)
		{
			var pptExportConfig = configurationControl as PowerPointExportConfig;
			fileName = pptExportConfig.Filename;
		}

		protected override void OnEnabling(EnablingEventArgs e)
		{
			if (!File.Exists(fileName))
			{
				ErrorLog.AddError(ErrorType.Failure, Strings.General_FileNotFound);
				e.Cancel = true;
			}
		}		

        public override void Perform()
        {
    		if (File.Exists(fileName))
			{
				try
				{
                    FileStream stream = File.Open(fileName, FileMode.Create);
                    streamWriter = new StreamWriter(stream);                   					
				}
				catch(Exception ex)
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
            catch (Exception e)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.PowerPoint_CantExportText);
                Logger.Write(e);
            }
            finally
            {
                app = null;
            }
        }

        #region IWpfConfigurable Methods

        public WpfConfiguration ConfigurationControl
        {
            get { return new PowerPointExportConfig(fileName); }
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
