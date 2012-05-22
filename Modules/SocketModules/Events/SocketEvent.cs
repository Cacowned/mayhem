using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using System.Net.Sockets;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.IO.Pipes;
using System.IO;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using SocketModules.Wpf;
using System.Runtime.Serialization;

namespace SocketModules.Events
{
	[DataContract]
	[MayhemModule("Socket Event", "Socket Description")]
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
