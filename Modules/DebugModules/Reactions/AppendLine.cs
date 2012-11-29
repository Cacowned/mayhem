using System;
using System.IO;
using System.Runtime.Serialization;
using DebugModules.Wpf;
using MayhemCore;
using MayhemWpf.ModuleTypes;

namespace DebugModules.Reactions
{
    [DataContract]
    [MayhemModule("Debug: Append Line", "Appends a line to a .txt file")]
    public class AppendLine : ReactionBase, IWpfConfigurable
    {
        [DataMember]
        private string lineToAdd;

        [DataMember]
        private string filePath;

        public override void Perform()
        {
            StreamWriter stream;           
            
            try
            {
                stream = new StreamWriter(filePath, true);
                stream.WriteLine(lineToAdd);
                stream.Close();
            }
            catch
            {
                ErrorLog.AddError(ErrorType.Failure, "File is open or being used by multiple reactions");
            }            
        }

        public MayhemWpf.UserControls.WpfConfiguration ConfigurationControl
        {
            get { return new AppendLineConfig(lineToAdd, filePath); }
        }

        public void OnSaved(MayhemWpf.UserControls.WpfConfiguration configurationControl)
        {
            var config = (AppendLineConfig)configurationControl;
            lineToAdd = config.Line;
            filePath = config.File;

            StreamWriter timeLogWriter = new StreamWriter(filePath);
            timeLogWriter.Close();
        }

        protected override void OnAfterLoad()
        {
            filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Mayhem") + @"\AppendLog.txt";            
        }

        public string GetConfigString()
        {
            return "\"" + lineToAdd + "\" to File: " + Path.GetFileName(filePath);
        }
    }
}