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
        Input = 0x00,
        Output = 0x01,
        Analog = 0x02,
        Pwm = 0x03,
        Servo = 0x04,
        Shift = 0x05,
        I2C = 0x06,
        Unassigned = 0xff
    }
}
