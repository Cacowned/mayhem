using System;
using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using DefaultModules.Wpf;
using MayhemCore;
using MayhemCore.ModuleTypes;

namespace DefaultModules.Reactions
{
    [DataContract]
    public class RunProgram : ReactionBase, IWpf
    {
        private string _fileName;
        private string _arguments;
        [DataMember]
        private string FileName
        {
            get
            {
                return _fileName;
            }
            set {
                _fileName = value;
                SetConfigString();
            }
        }
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


        public RunProgram()
            : base("Run Program", "Runs a given program.")
        {
            hasConfig = true;
         
            // Set the default
            FileName = Path.Combine(Environment.GetEnvironmentVariable("Windir"), "System32", "calc.exe");
        }

        public override void Perform()
        {
            System.Diagnostics.Process.Start(FileName, Arguments);
        }

        public void WpfConfig()
        {
            var window = new RunProgramConfig(FileName, Arguments);
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            window.ShowDialog();

            if (window.DialogResult == true)
            {

                FileName = window.filename;
                Arguments = window.arguments;
            }
        }

        private void SetConfigString()
        {
            ConfigString = String.Format("Filename: \"{0}\"\nArguments: \"{1}\"", Path.GetFileName(FileName), Arguments);
        }
    }
}
