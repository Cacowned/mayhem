using System.IO.Ports;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemSerial;

namespace SerialModules.Reactions
{
	[DataContract]
	[MayhemModule("Serial Write", "Writes Serially")]
	public class SerialWrite : ReactionBase
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
			manager.ConnectPort("COM6", settings, RecievedData);
		}

		public override void Perform()
		{
			manager.Write("COM6", "Foo");
		}

		protected override void OnDisabled(DisabledEventArgs e)
		{
			manager.ReleasePort("COM6", RecievedData);
		}

		private void RecievedData(byte[] bytes, int numBytes)
		{
			//string str = Encoding.UTF8.GetString(bytes);

			//Logger.WriteLine(str);
		}
	}
}
