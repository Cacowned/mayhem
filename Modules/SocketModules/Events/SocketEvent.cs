using System.Runtime.Serialization;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using SocketModules.Wpf;

namespace SocketModules.Events
{
	[DataContract]
	[MayhemModule("MayhemEvent.exe", "Trigger this event using the MayhemEvent.exe executable")]
	public class SocketEvent : EventBase, IWpfConfigurable
	{
		[DataMember]
		private string phrase;

		protected override void OnEnabling(EnablingEventArgs e)
		{
			PipeServerManager.Listen(phrase, PhraseHeard);
		}

		private void PhraseHeard()
		{
			Trigger();
		}

		protected override void OnDisabled(DisabledEventArgs e)
		{
			PipeServerManager.Forget(phrase, PhraseHeard);
		}

		public WpfConfiguration ConfigurationControl
		{
			get
			{
				return new SocketEventConfig(phrase);
			}
		}

		public void OnSaved(WpfConfiguration configurationControl)
		{
			var config = configurationControl as SocketEventConfig;
			phrase = config.Phrase;
		}

		public string GetConfigString()
		{
			return phrase;
		}
	}
}
