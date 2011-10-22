using System.Runtime.Serialization;
using ArduinoModules.Firmata;

namespace ArduinoModules.Wpf.Helpers
{
    /// <summary>
    /// Data model for Gridviews on Arduino digital pins for ArduinoDigitalWriteConfig
    /// </summary>
    [DataContract]
    public class DigitalPinWriteItem
    {
        // if checked, output is activated on this pin 
        [DataMember]
        private bool active = false;

        public bool Active
        {
            get
            {
                return active;
            }
            set
            {
                active = value;
            }
        }

        [DataMember]
        private int firmataId = 0;

        public int GetPinId()
        {
            return firmataId;
        }

        [DataMember]
        private DigitalWriteMode writeMode;

        public DigitalWriteMode WriteMode
        {
            get { return writeMode; }
            set { writeMode = value; }
        }

        public string PinName
        {
            get { return "D" + firmataId; }
        }

        // state
        [DataMember]
        private int digitalPinState = 0;

        /// <summary>
        /// Explicit getter/setter implementation to avoid getting columnized
        /// </summary>
        /// <returns></returns>
        public int GetPinState()
        {
            return digitalPinState;
        }

        public int SetPinState(int state)
        {
            digitalPinState = state;
            return GetPinState();
        }           // also return an int for easier asssignment to arduino.digitalwrite

        public DigitalPinWriteItem(bool check, int id, DigitalWriteMode mode)
        {
            active = check;
            firmataId = id;
            WriteMode = mode;
        }
    }
}
