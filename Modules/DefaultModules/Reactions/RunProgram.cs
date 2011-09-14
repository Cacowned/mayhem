using System;
using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using DefaultModules.Wpf;
using MayhemCore;
using MayhemCore.ModuleTypes;
using System.Windows.Controls;
using MayhemDefaultStyles.UserControls;

namespace DefaultModules.Reactions
{
    [DataContract]
    [MayhemModuleAttribute("Run Program", "Runs a given program")]
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

        public override void Perform()
        {
            try
            {
                System.Diagnostics.Process.Start(FileName, Arguments);
            }
            catch
            {
                ErrorLog.AddError(ErrorType.Failure, "Could not start the application");
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
                ErrorLog.AddError(ErrorType.Failure, "Could not find the application:\n" + FileName);
            }
        }

        public override void SetConfigString()
        {
            ConfigString = String.Format("Filename: \"{0}\"\nArguments: \"{1}\"", Path.GetFileName(FileName), Arguments);
        }
    }
}
