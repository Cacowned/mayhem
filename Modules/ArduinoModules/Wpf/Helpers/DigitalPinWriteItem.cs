using System.Runtime.Serialization;
/*
 * DitialPinItem.cs
 * 
 * 
 * Data model for Gridviews on Arduino digital pins for ArduinoDigitalWriteConfig
 * 
 * 
 * (c) 2011, Microsoft Applied Sciences Group
 * 
 * Author: Sven Kratz
 * 
 * 
 */
using ArduinoModules.Firmata;


namespace ArduinoModules.Wpf.Helpers
{
    [DataContract]
    public class DigitalPinWriteItem
    {
        // if checked, output is activated on this pin 
        [DataMember]
        private bool active_ = false;

        public bool Active
        {
            get { return active_; }
            set { active_ = value; }
        
        }

        [DataMember]
        private int firmata_id=0;

        public int GetPinID()
        {
            return firmata_id;
        }

        [DataMember]
        private DIGITAL_WRITE_MODE write_mode_; 
        public DIGITAL_WRITE_MODE WriteMode
        {
            get { return write_mode_; }
            set { write_mode_ = value; }
        }

        
        public string PinName
        {
            get { return "D" + firmata_id; }
        }


        // state
        [DataMember]
        private int digitalPinState_ = 0;
        /// <summary>
        /// Explicit getter/setter implementation to avoid getting columnized
        /// </summary>
        /// <returns></returns>
        public int GetPinState() { return digitalPinState_; }
        public int SetPinState(int state) { digitalPinState_ = state; return GetPinState(); }           // also return an int for easier asssignment to arduino.digitalwrite

        public DigitalPinWriteItem(bool check, int id, DIGITAL_WRITE_MODE mode)
        {
            active_ = check;
            firmata_id = id;
            WriteMode = mode; 
        }
    }
}
