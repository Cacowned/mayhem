using System.IO.Ports;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemSerial;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using SerialModules.Wpf;

namespace SerialModules.Reactions
{
	[DataContract]
	[MayhemModule("Serial Write", "Writes Serially")]
	public class SerialWrite : ReactionBase, IWpfConfigurable
	{
		private SerialPortManager manager;

		[DataMember]
		private SerialSettings settings;

		[DataMember]
		private string port;

		[DataMember]
		private string phrase;

		protected override void OnLoadDefaults()
		{
			this.port = "COM1";
			this.phrase = string.Empty;
			this.settings = new SerialSettings(9600, Parity.Even, StopBits.One, 8);
		}

		protected override void OnAfterLoad()
		{
			this.manager = SerialPortManager.Instance;
		}

		protected override void OnEnabling(EnablingEventArgs e)
		{
			this.manager.ConnectPort(this.port, this.settings, action: null);
		}

		protected override void OnDisabled(DisabledEventArgs e)
		{
			this.manager.ReleasePort(this.port, action: null);
		}

		public override void Perform()
		{
			this.manager.Write(this.port, this.phrase);
		}

		#region IWpfConfigurable
		public WpfConfiguration ConfigurationControl
		{
			get
			{
				return new SerialWriteConfig(this.port, this.settings, this.phrase);
			}
		}

		public void OnSaved(WpfConfiguration configurationControl)
		{
			var config = configurationControl as SerialWriteConfig;
			this.port = config.Selector.PortName;
			this.settings = config.Selector.Settings;

			this.phrase = config.Phrase;
		}

		public string GetConfigString()
		{
			string shortPhrase = this.phrase;

			if (shortPhrase.Length > 10)
			{
				shortPhrase = string.Format("{0}...", shortPhrase.Substring(0, 10));
			}

			return string.Format("Writing '{0}' to {1}", shortPhrase, this.port);
		}
		#endregion
	}
}
