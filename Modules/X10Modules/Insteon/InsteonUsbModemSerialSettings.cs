using System.IO.Ports;
using MayhemSerial;

namespace X10Modules.Insteon
{
	/// <summary>
	/// Insteon USB Modem 
	/// </summary>
	public class InsteonUsbModemSerialSettings : SerialSettings
	{
		public InsteonUsbModemSerialSettings() :
			base(19200, Parity.None, StopBits.One, 8)
		{
		}
	}
}
