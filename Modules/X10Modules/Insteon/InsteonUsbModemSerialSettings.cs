using System.IO.Ports;
using MayhemSerial;

namespace X10Modules.Insteon
{
    /// <summary>
    /// Insteon USB Modem 
    /// </summary>
    public class InsteonUsbModemSerialSettings : SerialSettings
    {
        public override int BaudRate
        {
            get
            {
                return 19200;
            }
        }

        public override Parity Parity
        {
            get
            {
                return Parity.None;
            }
        }

        public override StopBits StopBits
        {
            get
            {
                return StopBits.One;
            }
        }

        public override int DataBits
        {
            get
            {
                return 8;
            }
        }
    }
}
