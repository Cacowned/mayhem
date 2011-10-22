using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArduinoModules.Firmata
{
    /// <summary>
    /// Describes the PIN MODE and also if it is analog or not used
    /// </summary>
    public enum PinMode
    {
        INPUT = 0x00,
        OUTPUT = 0x01,
        ANALOG = 0x02,
        PWM = 0x03,
        SERVO = 0x04,
        SHIFT = 0x05,
        I2C = 0x06,
        UNASSIGNED = 0xff
    }
}
