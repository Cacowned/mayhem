﻿/*
 * 
 * DitialPinItem.cs
 * 
 * 
 * Data model for Gridviews on Arduino digital pins for ArduinoEventConfig
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
using System.Runtime.Serialization;

namespace ArduinoModules.Wpf
{
    /// <summary>
    /// Items for Ditial pins ItemsControl 
    /// </summary>
    [DataContract]
    public class DigitalPinItem
    {
        // selected
        [DataMember]
        private bool isChecked = false;
        public bool Selected { get { return isChecked; } set { isChecked = value; } }

        // friendly name
        [DataMember]
        private string pinName_;
        public string PinName
        {
            get { return pinName_; }
        }

        // change type
        [DataMember]
        private DIGITAL_PIN_CHANGE monitor_pin_change_ { get; set; }
        public DIGITAL_PIN_CHANGE ChangeType { get { return monitor_pin_change_; } set { monitor_pin_change_ = value; } }

        [DataMember]
        private int firmata_pin_id_ = 0;
        public int GetPinID()
        {
            return firmata_pin_id_;
        }
        //public int pin_id { get { return pin_id_; } }

        // state
        [DataMember]
        private int  digitalPinState_ = 0;

        /// <summary>
        /// Explicit getter/setter implementation to avoid getting columnized
        /// </summary>
        /// <returns></returns>
        public int GetDigitalPinState(){ return digitalPinState_;}
       


        public string CurrentPinState
        {
            get
            {
                if (digitalPinState_>0)
                    return "HIGH";
                else
                    return "LOW";
            }
        }

        /// <summary>
        /// Set the digital pin state. Explicitly implemented setter, to be able to provide a string
        /// representation to a datagrid.
        /// </summary>
        /// <param name="state"></param>
        public void SetPinState(int state)
        {
            digitalPinState_ = state;
        }


        public DigitalPinItem(bool check, int id, DIGITAL_PIN_CHANGE change)
        {
            isChecked = check;
            pinName_ = "D" + id;
            monitor_pin_change_ = change;
            firmata_pin_id_ = id;
        }


    }
}
