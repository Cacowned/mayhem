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
        Toggle,         // pin toggles, initially set to 0
        High,           // pull pin high
        Low,            // pull low
        PulseOn,       // pull pin high for a short interval
        PulseOff       // pull pin low for a short interval
    }
}
