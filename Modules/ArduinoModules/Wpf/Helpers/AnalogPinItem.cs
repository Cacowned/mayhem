using System.Runtime.Serialization;
using ArduinoModules.Firmata;

namespace ArduinoModules.Wpf.Helpers
{
    /// <summary>
    /// Data model for Gridviews on Arduino analog pins
    /// Items for Ditial pins ItemsControl 
    /// </summary>
    [DataContract]
    public class AnalogPinItem
    {
        [DataMember]
        private static int analogPinId;

        // selected 
        [DataMember]
        private bool isChecked;

        public bool Selected
        {
            get
            {
                return isChecked;
            }
            set
            {
                isChecked = value;
            }
        }

        // friendly Name
        [DataMember]
        private string pinName;

        public string PinName
        {
            get
            {
                return pinName;
            }
        }

        // pin change type
        [DataMember]
        private AnalogPinChange MonitorPinChange
        {
            get;
            set;
        }

        public AnalogPinChange ChangeType
        {
            get
            {
                return MonitorPinChange;
            }
            set
            {
                MonitorPinChange = value;
            }
        }

        [DataMember]
        private int firmataPinId;

        [DataMember]
        private int setValue;

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

        [DataMember]
        private int analogValue;

        public int CurrentAnalogValue
        {
            get
            {
                return analogValue;
            }
        }

        public int GetPinId()
        {
            return firmataPinId;
        }

        public static void ResetAnalogIDs()
        {
            analogPinId = 0;
        }

        /// <summary>
        /// Sets the analog value for display
        /// Not implemented as setter for AnalogValue, as we want this to be read-only in the
        /// data grid.
        /// </summary>
        /// <param name="value"></param>
        public void SetAnalogValue(int value)
        {
            analogValue = value;
        }

        public AnalogPinItem(bool check, int id, AnalogPinChange change)
        {
            isChecked = check;
            pinName = "A" + analogPinId++;
            MonitorPinChange = change;
            firmataPinId = id;
            setValue = 512;
        }
    }
}
