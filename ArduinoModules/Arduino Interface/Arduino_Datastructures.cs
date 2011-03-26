using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArduinoModules.Arduino_Interface
{
    public enum PinMode { INPUT, OUTPUT };

    public struct DigitalIO
    {
        public PinMode pin_mode = PinMode.INPUT;
        public bool pin_state = false;
    }

    public struct AnalogInput
    {
        public PinMode pin_mode = PinMode.INPUT;
        // 16 bit sampling
        public UInt16 value = 0; 
    }

    public struct AnalogOutput
    {
        public PinMode pin_mode = PinMode.OUTPUT;
        public UInt16 value = 0; 
    }
}
