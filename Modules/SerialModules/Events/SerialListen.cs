using System.IO.Ports;
using System.Runtime.Serialization;
using System.Text;
using MayhemCore;
using MayhemSerial;

namespace SerialModules.Events
{
	[DataContract]
	[MayhemModule("Serial Listen", "Listens Serially")]
	public class SerialListen : EventBase
	{
		SerialPortManager manager;
		SerialSettings settings;

		protected override void OnAfterLoad()
		{
			manager = SerialPortManager.Instance;
			settings = new SerialSettings(9600, Parity.Even, StopBits.One, 8);
		}

		protected override void OnEnabling(EnablingEventArgs e)
		{
			manager.ConnectPort("COM7", settings, RecievedData);
		}

		protected override void OnDisabled(DisabledEventArgs e)
		{
			manager.ReleasePort("COM7", RecievedData);
		}

		private void RecievedData(byte[] bytes, int numBytes) 
		{
			string str = Encoding.UTF8.GetString(bytes);

			Logger.WriteLine(str);
		}
	}
}
