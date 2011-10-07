/*
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

using System.Runtime.Serialization;
using ArduinoModules.Firmata;

namespace ArduinoModules.Wpf.Helpers
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
        private string pinName;
        public string PinName
        {
            get { return pinName; }
        }

        // change type
        [DataMember]
        private DIGITAL_PIN_CHANGE MonitorPinChange { get; set; }
        public DIGITAL_PIN_CHANGE ChangeType { get { return MonitorPinChange; } set { MonitorPinChange = value; } }

        [DataMember]
        private int firmataPinId = 0;
        public int GetPinId()
        {
            return firmataPinId;
        }
        //public int pin_id { get { return pin_id_; } }

        // state
        [DataMember]
        private int digitalPinState = 0;

        /// <summary>
        /// Explicit getter/setter implementation to avoid getting columnized
        /// </summary>
        /// <returns></returns>
        public int GetDigitalPinState() { return digitalPinState; }



        public string CurrentPinState
        {
            get
            {
                if (digitalPinState > 0)
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
            digitalPinState = state;
        }


        public DigitalPinItem(bool check, int id, DIGITAL_PIN_CHANGE change)
        {
            isChecked = check;
            pinName = "D" + id;
            MonitorPinChange = change;
            firmataPinId = id;
        }


    }
}
