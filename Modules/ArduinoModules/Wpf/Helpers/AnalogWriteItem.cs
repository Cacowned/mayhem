using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace ArduinoModules.Wpf.Helpers
{
    [DataContract]
    public class AnalogWriteItem
    {
        [DataMember]
        private bool active = false;

        [DataMember]
        public Int16 AnalogWriteValue
        {
            get;
            set;
        }

        [DataMember]
        public int PinId
        {
            get;
            private set;
        }

        public string PinName
        {
            get { return "D(W)" + PinId; }
        }

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

        public AnalogWriteItem(int id, Int16 writeValue)
        {
            PinId = id;
            AnalogWriteValue = writeValue;
        }

    }
}
