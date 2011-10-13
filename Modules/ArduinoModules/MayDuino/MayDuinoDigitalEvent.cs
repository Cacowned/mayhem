using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using System.Runtime.Serialization;
using ArduinoModules.Wpf;
using MayhemWpf.UserControls;
using ArduinoModules.MayDuino;

namespace ArduinoModules.Events
{
    [DataContract]
    [MayhemModule("MayDuino Digital Event", "Event Setup for MayDuino Core")]
    public class MayDuinoDigitalEvent : MayduinoEventBase, IWpfConfigurable
    {
        [DataMember]
        private int digitalPin;

        [DataMember]
        private bool condition;

        [DataMember]
        private bool pullUp; 

        protected override Event_t EventType
        {
            get
            {
                return Event_t.DIGITALEVENT;
            }
        }

        protected override void OnLoadDefaults()
        {
            digitalPin = 2;
            condition = true;
            pullUp = true;
        }
         
        protected override void OnEnabling(EnablingEventArgs e)
        {
            manager.EventEnabled(this);
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            manager.EventDisabled(this);
        }

        public MayhemWpf.UserControls.WpfConfiguration ConfigurationControl
        {
            get 
            {
                return new MayDuinoDigitalEventConfig(digitalPin, condition, pullUp);
            }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
           // throw new NotImplementedException();
            MayDuinoDigitalEventConfig config = configurationControl as MayDuinoDigitalEventConfig;

            digitalPin = config.Pin;
            condition = config.GetCondition<bool>();
            pullUp = config.GetPullup<bool>();
        }     

        public string GetConfigString()
        {
            return "MayDuino Digital Event";
        }

        public override string EventConfigString
        {
            get
            {
                //return base.GetEventConfigString();
                string ec = digitalPin + "," + (int)EventType + "," + Convert.ToInt32(condition) + "," + Convert.ToInt32(pullUp);
                return ec;
            }
        }
    }
}
