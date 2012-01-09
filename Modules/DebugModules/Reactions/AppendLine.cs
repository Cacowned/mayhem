using System;
using System.IO;
using System.Runtime.Serialization;
using DebugModules.Wpf;
using MayhemCore;
using MayhemWpf.ModuleTypes;

namespace DebugModules.Reactions
{
    [DataContract]
    [MayhemModule("Append Line", "Appends a line to a .txt file")]
    public class AppendLine : ReactionBase, IWpfConfigurable
    {
        [DataMember]
        private string lineToAdd;

        [DataMember]
        private string filePath;

        public override void Perform()
        {
            if (File.Exists(filePath))
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath, true))
                {
                    file.WriteLine(lineToAdd);
                }
            }
            else
            {
                ErrorLog.AddError(ErrorType.Failure, "File does not exist");
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
        }

        public string GetConfigString()
        {
            return "\"" + lineToAdd + "\" to File: " + Path.GetFileName(filePath);
        }
    }
}
