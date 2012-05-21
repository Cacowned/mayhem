using System.Globalization;
using System.IO;
using System.Runtime.Serialization;
using DefaultModules.Resources;
using DefaultModules.Wpf;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using System.Diagnostics;

namespace DefaultModules.Reactions
{
	[DataContract]
	[MayhemModule("Run Program", "Runs a given program")]
	public class RunProgram : ReactionBase, IWpfConfigurable
	{
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

		public override void Perform()
		{
			if (File.Exists(FileName))
			{
				try
				{
					ProcessStartInfo startInfo = new ProcessStartInfo();
					startInfo.WorkingDirectory = new DirectoryInfo(FileName).Parent.FullName;
					startInfo.FileName = FileName;
					startInfo.Arguments = Arguments;
					
					Process.Start(startInfo);
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
			var rpc = configurationControl as RunProgramConfig;
			FileName = rpc.Filename;
			Arguments = rpc.Arguments;
		}

		protected override void OnEnabling(EnablingEventArgs e)
		{
			if (!File.Exists(FileName))
			{
				ErrorLog.AddError(ErrorType.Failure, Strings.RunProgram_FileNotFound);
				e.Cancel = true;
			}
		}

		public string GetConfigString()
		{
			return string.Format(CultureInfo.CurrentCulture, Strings.RunProgram_ConfigString, Path.GetFileName(FileName));
		}
	}
}
