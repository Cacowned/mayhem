using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArduinoModules.Firmata
{
    /// <summary>
    /// Types of digital pin changes that can be detected
    /// </summary>
    public enum DigitalPinChange
    {
        High,
        Low,
        Rising,
        Falling
    }
}
