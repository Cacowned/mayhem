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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArduinoModules.Firmata;

namespace ArduinoModules.Wpf.Helpers
{
    public class DigitalPinWriteItem
    {
        private bool active_;
        public bool Active { get; set; }


        private int firmata_id=0;
        public int GetPinID()
        {
            return firmata_id;
        }

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
        private int digitalPinState_ = 0;
        /// <summary>
        /// Explicit getter/setter implementation to avoid getting columnized
        /// </summary>
        /// <returns></returns>
        public int GetPinState() { return digitalPinState_; }
        public int SetPinState(int state) { digitalPinState_ = state; return GetPinState(); }           // also return an int for easier asssignment to arduino.digitalwrite

        public DigitalPinWriteItem(bool check, int id, DIGITAL_WRITE_MODE mode)
        {
            Active = check;
            firmata_id = id;
            WriteMode = mode; 
        }
    }
}
