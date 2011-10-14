using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArduinoModules.Events;
using MayhemWpf.ModuleTypes;
using System.Runtime.Serialization;
using MayhemCore;
using ArduinoModules.Wpf;

namespace ArduinoModules.MayDuino
{
    [DataContract]
    [MayhemModule("MayDuino Analog Event", "Analog event for MayDuino Core")]
    public class MayDuinoAnalogEvent : MayduinoEventBase, IWpfConfigurable
    {

        [DataMember]
        private int analogPin;              // the pin id

        [DataMember]
        private bool condition;             // lower or higher

        [DataMember]
        private int threshold;             // threshold value        

        protected override Event_t EventType
        {
            get
            {
                return Event_t.ANALOGEVENT;
            }
        }

        public MayhemWpf.UserControls.WpfConfiguration ConfigurationControl
        {
            get 
            { 
                return new MayDuinoAnalogEventConfig(analogPin, condition, threshold);
            }
        }

        public void OnSaved(MayhemWpf.UserControls.WpfConfiguration configurationControl)
        {
            MayDuinoDigitalEventConfig config = configurationControl as MayDuinoDigitalEventConfig;
            analogPin = config.Pin;
            condition = config.GetCondition<bool>();
            threshold = config.ActivationValue; 
        }

        public string GetConfigString()
        {   
            string condTxt = condition ? "GREATER" : "SMALLER";
            return "Pin " + analogPin + " " + condTxt + " " + threshold; 
        }

        public override string EventConfigString
        {
            get
            {
                string ec = analogPin + "," + (int)EventType + "," + Convert.ToInt32(condition) + "," + threshold;
                return ec;
            }
        }

    }
}
