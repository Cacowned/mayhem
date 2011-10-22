using System.IO.Ports;
using MayhemSerial;

namespace ArduinoModules.Firmata
{
    /// <summary>
    /// Arduino running Firmata Firmware
    /// </summary>
    public class ArduinoFirmataSerialSettings : SerialSettings
    {
        public override int BaudRate
        {
            get { return 57600; }
        }

        public override Parity Parity
        {
            get { return Parity.None; }
        }

        public override StopBits StopBits
        {
            get { return StopBits.One; }
        }

        public override int DataBits
        {
            get { return 8; }
        }
    }
}
