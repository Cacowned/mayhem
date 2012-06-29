using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using DebugModules.Wpf;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;

namespace DebugModules.Reactions
{
    [DataContract]
    [MayhemModule("Time Log", "Logs the event time to a .txt file")]
    public class TimeLog : ReactionBase, IWpfConfigurable
    {
        [DataMember]
        private string filePath;        

        public override void Perform()
        {
            if (File.Exists(filePath))
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(filePath, true))
                {
                    file.WriteLine(DateTime.Now.ToString(@"yyyy\/MM\/dd HH\:mm\:ss\.fff"));
                }
            }

            else
            {
                ErrorLog.AddError(ErrorType.Failure, "File does not exist");
            }
        }

        public MayhemWpf.UserControls.WpfConfiguration ConfigurationControl
        {   
            get { return new TimeLogConfig(filePath); }
        }

        public void OnSaved(MayhemWpf.UserControls.WpfConfiguration configurationControl)
        {
            var config = (TimeLogConfig)configurationControl;
            filePath = config.File;
        }

        protected override void OnAfterLoad()
        {            
            filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Mayhem") + @"\TimeLog.txt";            
            StreamWriter timeLogWriter = new StreamWriter(filePath);                        
        }

        public string GetConfigString()
        {
            return "Logs the event time to: " + Path.GetFileName(filePath);
        }
    }
}