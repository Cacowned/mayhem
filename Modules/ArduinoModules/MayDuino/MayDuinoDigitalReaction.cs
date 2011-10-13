using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using System.Runtime.Serialization;
using ArduinoModules.Wpf;
using MayhemWpf.UserControls;

namespace ArduinoModules.Events
{
    [DataContract]
    [MayhemModule("MayhDuino Digital Pin Reaction", "Writes logic values on selected pins when triggered")]
    public class MayDuinoDigitalReaction : MayduinoReactionBase, IWpfConfigurable
    {
        [DataMember]
        private int pin;

        [DataMember]
        private bool outState;

        protected override void OnEnabling(EnablingEventArgs e)
        {
            manager.ReactionEnabled(this);
        }

        public MayhemWpf.UserControls.WpfConfiguration ConfigurationControl
        {
            get 
            { 
                return new MayDuinoDigitalReactionConfig(); 
            }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
           // throw new NotImplementedException();
            MayDuinoDigitalReactionConfig config = configurationControl as MayDuinoDigitalReactionConfig;

            pin = config.Pin;
            outState = (bool)config.Condition;
        }

        public string GetConfigString()
        {
            return "Mayduino digital reactiomn";
        }

        public override void Perform()
        {
            //throw new NotImplementedException();
        }

        protected override string GetReactionConfigString()
        {
            throw new NotImplementedException();
        }
    }
}
