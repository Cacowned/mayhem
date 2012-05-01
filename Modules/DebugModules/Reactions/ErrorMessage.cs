using System;
using System.Globalization;
using System.Runtime.Serialization;
using DebugModules.Wpf;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;

namespace DebugModules.Reactions
{
	[DataContract]
	[MayhemModule("Debug: Error Log", "Writes a message to the Error Log")]
	public class ErrorMessage : ReactionBase, IWpfConfigurable
	{
		[DataMember]
		private string MessageText { get; set; }

		public override void Perform()
		{
			ErrorLog.AddError(ErrorType.Message, string.Format(CultureInfo.CurrentCulture, "Message: {0}", MessageText));
		}

		public void OnSaved(WpfConfiguration configurationControl)
		{
			MessageText = ((DebugMessageConfig)configurationControl).Message;
		}

		public string GetConfigString()
		{
			return MessageText;
		}

		public WpfConfiguration ConfigurationControl
		{
			get { return new DebugMessageConfig(MessageText); }
		}
	}
}
