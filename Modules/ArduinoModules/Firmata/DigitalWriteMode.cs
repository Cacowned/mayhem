using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArduinoModules.Firmata
{
    /// <summary>
    /// Digital pin write modes for use by Mayhem
    /// </summary>
    public enum DigitalWriteMode
    {
        TOGGLE,         // pin toggles, initially set to 0
        HIGH,           // pull pin high
        LOW,            // pull low
        PULSE_ON,       // pull pin high for a short interval
        PULSE_OFF       // pull pin low for a short interval
    }
}
