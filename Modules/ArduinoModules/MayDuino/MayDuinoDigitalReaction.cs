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
    [MayhemModule("MayhDuino Digital Pin Reaction", "Writes logic values on selected pins when triggered")]
    public class MayDuinoDigitalReaction : MayduinoReactionBase, IWpfConfigurable
    {
        [DataMember]
        private int digitalPin;

        [DataMember]
        private bool outState;

        protected override Reaction_t ReactionType
        {
            get
            {
                return Reaction_t.DIGITALREACTION ;
            }
        }

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

            digitalPin = config.Pin;
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
            //return base.GetEventConfigString();
            string rc = digitalPin + "," + (int)ReactionType + "," + Convert.ToInt32(outState) + "," + 0;
            return rc;
        }
    }
}
