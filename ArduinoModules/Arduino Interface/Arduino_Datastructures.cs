using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArduinoModules.Arduino_Interface
{
    public enum PinMode { INPUT, OUTPUT };

    public struct DigitalPin
    {
        public PinMode pin_mode;
        public bool pin_state;

        public DigitalPin()
        {
            pin_mode = PinMode.INPUT;
            pin_state = false;
        }
    }

    public struct AnalogIn
    {
        public PinMode pin_mode = PinMode.INPUT;
        // 16 bit sampling
        public UInt16 value = 0; 
 
    }
}
