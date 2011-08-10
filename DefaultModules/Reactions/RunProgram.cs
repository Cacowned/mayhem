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
    [MayhemModule("Run Program", "Runs a given program")]
    public class RunProgram : ReactionBase, IWpfConfigurable
    {
        #region Configuration Properties
        private string _fileName;
        [DataMember]
        private string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
                SetConfigString();
            }
        }

        private string _arguments;
        [DataMember]
        private string Arguments
        {
            get
            {
                return _arguments;
            }
            set
            {
                _arguments = value;
                SetConfigString();
            }
        }
        #endregion

        public RunProgram()
        { }

        protected override void Initialize()
        {
            base.Initialize();

            // Set the default
            FileName = Path.Combine(Environment.GetEnvironmentVariable("Windir"), "System32", "calc.exe");
        }

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

        public override void SetConfigString()
        {
            ConfigString = String.Format("Filename: \"{0}\"\nArguments: \"{1}\"", Path.GetFileName(FileName), Arguments);
        }
    }
}
