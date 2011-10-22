using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ArduinoModules.Firmata
{
    /// <summary>
    /// Represents an  arduino pin
    /// </summary>
    public class Pin
    {
        public PinMode Mode
        {
            get;
            set;
        }

        public byte AnalogChannel
        {
            get;
            set;
        }

        public UInt64 SupportedModes
        {
            get;
            set;
        }

        public int Value
        {
            get;
            set;
        }

        public int Id
        {
            get;
            set;
        }

        // Used by a module
        public bool Flagged
        {
            get;
            set;
        }

        public Pin()
        {
            Flagged = false;
        }
    }
}
