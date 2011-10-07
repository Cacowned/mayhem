using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemSerial;
using System.IO.Ports;

namespace X10Modules.Insteon
{
    /// <summary>
    /// Insteon USB Modem 
    /// </summary>
    public class InsteonUsbModemSerialSettings : SerialSettings
    {
        public override int BaudRate { get { return 19200; } }
        public override Parity Parity { get { return Parity.None; } }
        public override StopBits StopBits { get { return StopBits.One; } }
        public override int DataBits { get { return 8; } }
    }
}
