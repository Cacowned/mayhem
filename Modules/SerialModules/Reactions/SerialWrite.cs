using System.IO.Ports;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemSerial;
using System.Text;

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
			settings = new SerialSettings(19200, Parity.None, StopBits.One, 8);
		}

		protected override void OnEnabling(EnablingEventArgs e)
		{
			manager.ConnectPort("COM3", settings, null);
		}

		public override void Perform()
		{
			byte[] buffer = new byte[] { 0x02, 0x62, 0x15, 0xf3, 0x89, 0x00, 0x14, 0x00 };
			manager.Write("COM3", buffer, buffer.Length);
		}

		protected override void OnDisabled(DisabledEventArgs e)
		{
			manager.ReleasePort("COM3", null);
		}

		private void RecievedData(byte[] bytes, int numBytes)
		{
			string str = Encoding.UTF8.GetString(bytes);

			Logger.WriteLine(str);
		}
	}
}
