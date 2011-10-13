using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using System.Runtime.Serialization;
using ArduinoModules.Wpf;

namespace ArduinoModules.Events
{
    [DataContract]
    [MayhemModule("MayDuino Digital Event", "Event Setup for MayDuino Core")]
    public class MayDuinoDigitalEvent : EventBase, IWpfConfigurable
    {
        protected override void OnEnabling(EnablingEventArgs e)
        {
         
        }

        public MayhemWpf.UserControls.WpfConfiguration ConfigurationControl
        {
            get 
            {
                return new MayDuinoDigitalEventConfig();
            }
        }

        public void OnSaved(MayhemWpf.UserControls.WpfConfiguration configurationControl)
        {
           // throw new NotImplementedException();
        }

        public string GetConfigString()
        {
            //throw new NotImplementedException();
            return "MayDuino Digital";
        }
    }
}
