using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using DefaultModules.Resources;
using DefaultModules.Wpf;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;

namespace DefaultModules.Reactions
{
    [DataContract]
    [MayhemModule("Run Program", "Runs a given program")]
    public class RunProgram : ReactionBase, IWpfConfigurable
    {
        #region Configuration Properties
        [DataMember]
        private string FileName
        {
            get;
            set;
        }

        [DataMember]
        private string Arguments
        {
            get;
            set;
        }
        #endregion

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public override void Perform()
        {
            if (File.Exists(FileName))
            {
                try
                {
                    System.Diagnostics.Process.Start(FileName, Arguments);
                }
                catch
                {
                    ErrorLog.AddError(ErrorType.Failure, Strings.RunProgram_CantStartProgram);
                }
            }
            else
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.RunProgram_FileNotFound);
            }
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new RunProgramConfig(FileName, Arguments); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            RunProgramConfig rpc = configurationControl as RunProgramConfig;
            FileName = rpc.Filename;
            Arguments = rpc.Arguments;
        }

        protected override bool OnEnable()
        {
            if (File.Exists(FileName))
            {
                return true;
            }
            else
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.RunProgram_FileNotFound);
            }
            return false;
        }

        public string GetConfigString()
        {
            return String.Format(CultureInfo.CurrentCulture, Strings.RunProgram_ConfigString, Path.GetFileName(FileName), Arguments);
        }
    }
}
