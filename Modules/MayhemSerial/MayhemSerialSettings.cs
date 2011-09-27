/*
 * MayhemSerialSettings.cs
 * 
 * Settings database for various serial periphery used by Mayhem 
 * 
 * (c) 2011, Microsoft Applied Sciences Group
 * 
 * Author: Sven Kratz
 */ 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO.Ports;

namespace MayhemSerial
{
    public abstract  class SERIAL_SETTINGS {
        public abstract int baudRate { get;  }
        public abstract Parity parity { get; }
        public abstract StopBits stopBits { get; }
        public abstract int dataBits { get;  }
    };


    /// <summary>
    /// Insteon USB Modem 
    /// </summary>
    public class INSTEON_USB_MODEM_SETTINGS : SERIAL_SETTINGS
    {
        public override int baudRate { get { return 19200; } }
        public override Parity parity { get { return Parity.None; } }
        public override StopBits stopBits { get { return StopBits.One; } }
        public override int dataBits { get { return 8; } }
    }

    /// <summary>
    /// Arduino running Firmata Firmware
    /// </summary>
    public class ARDUINO_FIRMATA_SETTINGS : SERIAL_SETTINGS
    {
        public override int baudRate { get { return 57600; } }
        public override Parity parity { get { return Parity.None; } }
        public override StopBits stopBits { get { return StopBits.One; } }
        public override int dataBits { get { return 8; } }
    }

}
