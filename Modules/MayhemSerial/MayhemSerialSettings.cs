/*
 * MayhemSerialSettings.cs
 * 
 * Settings database for various serial periphery used by Mayhem 
 * 
 * (c) 2011, Microsoft Applied Sciences Group
 * 
 * Author: Sven Kratz
 */
using System.IO.Ports;

namespace MayhemSerial
{
    public abstract class SerialSettings {
        public abstract int BaudRate { get;  }
        public abstract Parity Parity { get; }
        public abstract StopBits StopBits { get; }
        public abstract int DataBits { get;  }
    };


    /// <summary>
    /// Insteon USB Modem 
    /// </summary>
    public class InsteonUsbModemSettings : SerialSettings
    {
        public override int BaudRate { get { return 19200; } }
        public override Parity Parity { get { return Parity.None; } }
        public override StopBits StopBits { get { return StopBits.One; } }
        public override int DataBits { get { return 8; } }
    }

    /// <summary>
    /// Arduino running Firmata Firmware
    /// </summary>
    public class ArduinoFirmataSettings : SerialSettings
    {
        public override int BaudRate { get { return 57600; } }
        public override Parity Parity { get { return Parity.None; } }
        public override StopBits StopBits { get { return StopBits.One; } }
        public override int DataBits { get { return 8; } }
    }

}
