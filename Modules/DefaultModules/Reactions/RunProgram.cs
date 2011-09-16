using System;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using DefaultModules.Resources;
using DefaultModules.Wpf;
using MayhemCore;
using MayhemCore.ModuleTypes;
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
            try
            {
                System.Diagnostics.Process.Start(FileName, Arguments);
            }
            catch
            {
                ErrorLog.AddError(ErrorType.Failure, EnglishStrings.RunProgram_CantStartProgram);
            }
        }

        public IWpfConfiguration ConfigurationControl
        {
            get { return new RunProgramConfig(FileName, Arguments); }
        }

        public void OnSaved(IWpfConfiguration configurationControl)
        {
            RunProgramConfig rpc = configurationControl as RunProgramConfig;
            FileName = rpc.Filename;
            Arguments = rpc.Arguments;
        }

        public override void Enable()
        {
            if (File.Exists(FileName))
            {
                base.Enable();
            }
            else
            {
                ErrorLog.AddError(ErrorType.Failure, EnglishStrings.RunProgram_FileNotFound);
            }
        }

        public override void SetConfigString()
        {
            ConfigString = String.Format(CultureInfo.CurrentCulture, EnglishStrings.RunProgram_ConfigString, Path.GetFileName(FileName), Arguments);
        }
    }
}
