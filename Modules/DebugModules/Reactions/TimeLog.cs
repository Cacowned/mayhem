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
    [MayhemModule("Debug: Time Log", "Logs the event time to a .txt file")]
    public class TimeLog : ReactionBase, IWpfConfigurable
    {
        [DataMember]
        private string filePath;        

        public override void Perform()
        {
            StreamWriter stream;

            try
            {
                stream = new StreamWriter(filePath, true);
                stream.WriteLine(DateTime.Now.ToString(@"yyyy\/MM\/dd HH\:mm\:ss\.fff"));
                stream.Close();
            }
            catch
            {
                ErrorLog.AddError(ErrorType.Failure, "File is open or being used by multiple reactions");
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

            StreamWriter timeLogWriter = new StreamWriter(filePath);
            timeLogWriter.Close();
        }

        protected override void OnAfterLoad()
        {            
            filePath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Mayhem") + @"\TimeLog.txt";                        
        }

        public string GetConfigString()
        {
            return "Logs the event time to: " + Path.GetFileName(filePath);
        }
    }
}