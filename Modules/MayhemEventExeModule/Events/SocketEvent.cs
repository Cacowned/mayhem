﻿using System.Runtime.Serialization;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using MayhemEventExeModule.Wpf;

namespace MayhemEventExeModule.Events
{
	[DataContract]
	[MayhemModule("MayhemEvent.exe", "Trigger this event using the MayhemEvent.exe executable")]
	public class MayhemEvent : EventBase, IWpfConfigurable
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
				return new MayhemEventConfig(phrase);
			}
		}

		public void OnSaved(WpfConfiguration configurationControl)
		{
			var config = configurationControl as MayhemEventConfig;
			phrase = config.Phrase;
		}

		public string GetConfigString()
		{
			return phrase;
		}
	}
}
