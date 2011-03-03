using System;
using System.IO;
using System.Runtime.Serialization;
using System.Windows;
using DefaultModules.Wpf;
using MayhemCore;
using MayhemCore.ModuleTypes;

namespace DefaultModules.Reactions
{
	[Serializable]
	public class RunProgram : ReactionBase, IWpf, ISerializable
	{
		protected string filename = "";
		protected string arguments = "";

		public RunProgram()
			: base("Run Program", "Runs a given program.") {

			filename = Path.Combine(Environment.GetEnvironmentVariable("Windir"), "System32", "calc.exe");

			Setup();

		}

		public void Setup() {
			hasConfig = true;
			SetConfigString();
		}

		public override void Perform() {

			System.Diagnostics.Process.Start(filename, arguments);
		}

		public void WpfConfig() {
			var window = new RunProgramConfig(filename, arguments);
			window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

			window.ShowDialog();

			if (window.DialogResult == true) {

				filename = window.filename;
				arguments = window.arguments;

				SetConfigString();
			}
		}

		private void SetConfigString() {
			ConfigString = String.Format("Filename: \"{0}\"\nArguments: \"{1}\"", Path.GetFileName(filename), arguments);
		}

		#region Serialization
		public RunProgram(SerializationInfo info, StreamingContext context)
			: base(info, context) {

			filename = info.GetString("FileName");
			filename = info.GetString("Arguments");

			Setup();
		}

		public new void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);
			info.AddValue("FileName", filename);
			info.AddValue("Arguments", arguments);
		}
		#endregion
	}
}
