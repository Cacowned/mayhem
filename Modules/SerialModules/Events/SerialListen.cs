using System;
using System.IO.Ports;
using System.Runtime.Serialization;
using System.Text;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using SerialManager;
using SerialModules.Wpf;

namespace SerialModules.Events
{
	[DataContract]
	[MayhemModule("Serial Listen", "Listens Serially")]
	public class SerialListen : EventBase, IWpfConfigurable
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
			try
			{
				this.manager.ConnectPort(this.port, this.settings, this.RecievedData);
			}
			catch (Exception ex)
			{
				ErrorLog.AddError(ErrorType.Failure, ex.Message);
				e.Cancel = true;
				return;
			}
		}

		protected override void OnDisabled(DisabledEventArgs e)
		{
			try
			{
				this.manager.ReleasePort(this.port, this.RecievedData);
			}
			catch
			{
				/* Swallow the exception because the only times this will happen is if we
				 * try to close a port that hasn't been opened or if it hasn't been connected with the specified action. 
				 * Aka, it was closed here before it was opened. Nothing we can do, might as well hide it
				 */
			}
		}

		private void RecievedData(byte[] bytes, int numBytes)
		{
			string str = Encoding.UTF8.GetString(bytes);

			if (str == this.phrase)
			{
				Trigger();
			}
		}

		#region IWpfConfigurable
		public WpfConfiguration ConfigurationControl
		{
			get
			{
				return new SerialListenConfig(this.port, this.settings, this.phrase);
			}
		}

		public void OnSaved(WpfConfiguration configurationControl)
		{
			var config = configurationControl as SerialListenConfig;
			this.port = config.Selector.PortName;
			this.settings = config.Selector.Settings;

			this.phrase = config.Phrase;
		}

		public string GetConfigString()
		{
			return string.Format("Listening for '{0}' on {1}", this.phrase, this.port);
		}
		#endregion
	}
}
