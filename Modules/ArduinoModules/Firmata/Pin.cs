using System;

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

        public ulong SupportedModes
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
