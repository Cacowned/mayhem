﻿/*
 * 
 * DitialPinItem.cs
 * 
 * 
 * Data model for Gridviews on Arduino analog pins
 * 
 * 
 * (c) 2011, Microsoft Applied Sciences Group
 * 
 * Author: Sven Kratz
 * 
 * 
 */

using System.Runtime.Serialization;
using ArduinoModules.Firmata;

namespace ArduinoModules.Wpf
{
    /// <summary>
    /// Items for Ditial pins ItemsControl 
    /// </summary>
    [DataContract]
    public class AnalogPinItem
    {
        [DataMember]
        private static int analog_pin_id = 0;

        // selected 
        [DataMember]
        private bool isChecked;
        public bool Selected { get { return isChecked; } set { isChecked = value; } }


        // friendly Name
        [DataMember]
        private string pinName_;
        public string PinName
        {
            get { return pinName_; }
        }

        // pin change type
        [DataMember]
        private ANALOG_PIN_CHANGE monitor_pin_change { get; set; }
        public ANALOG_PIN_CHANGE ChangeType { get { return monitor_pin_change; } set { monitor_pin_change = value; } }

        [DataMember]
        private int firmata_pin_id_ = 0;
        public int GetPinID()
        {
            return firmata_pin_id_;
        }
        //public int pin_id { get { return pin_id_; } }

        [DataMember]
        private int setValue = 0;

        public static void ResetAnalogIDs()
        {
            analog_pin_id = 0;
        }

        // change threshold value set by user
        public int SetValue
        {
            get
            {
                return setValue;
            }

            // constrain the settable value to a 16 bit range
            set
            {
                if (value >= 0 && value <= 1024)
                {
                    setValue = value;
                }
                else
                {
                    if (value <= 0)
                        setValue = 0;
                    else if (value >= 1024)
                        setValue = 1024;
                }
            }
        }


        // analog value
        [DataMember]
        private int aValue = 0;
        public int CurrentAnalogValue { get { return aValue; } }

        /// <summary>
        /// Sets the analog value for display
        /// Not implemented as setter for AnalogValue, as we want this to be read-only in the
        /// data grid.
        /// </summary>
        /// <param name="value"></param>
        public void SetAnalogValue(int value)
        {
            aValue = (int)value;
        }

        public AnalogPinItem(bool check, int id, ANALOG_PIN_CHANGE change)
        {
            isChecked = check;
            pinName_ = "A" + analog_pin_id++;
            monitor_pin_change = change;
            firmata_pin_id_ = id;
            setValue = 512;
        }
    }
}
