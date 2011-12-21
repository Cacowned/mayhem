using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using MayhemCore;
using System.Windows;
using MayhemWpf.ModuleTypes;
using System.IO;
using DebugModules.Wpf;

namespace DebugModules.Reactions
{
    [DataContract]
    [MayhemModule("Append Line", "Appends a line to a file")]
    public class AppendLine : ReactionBase, IWpfConfigurable
    {
        [DataMember]
        private String LineToAdd;
        [DataMember]
        private String FilePath;

        public override void Perform()
        {

            if (File.Exists(FilePath))
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@FilePath, true))
                {
                    file.WriteLine(LineToAdd);
                }
            }
            else
            {
                ErrorLog.AddError(ErrorType.Failure, "File does not exist");
            }
        }

        public MayhemWpf.UserControls.WpfConfiguration ConfigurationControl
        {
            get { return new AppendLineConfig(LineToAdd, FilePath); }
        }

        public void OnSaved(MayhemWpf.UserControls.WpfConfiguration configurationControl)
        {
            LineToAdd = ((AppendLineConfig)configurationControl).Line;
            FilePath = ((AppendLineConfig)configurationControl).File;
        }

        public string GetConfigString()
        {
            return "\"" + LineToAdd + "\" to File: " + Path.GetFileName(FilePath);
        }
    }
}
