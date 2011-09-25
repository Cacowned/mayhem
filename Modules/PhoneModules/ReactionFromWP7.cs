using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemCore.ModuleTypes;
using MayhemWpf.UserControls;

namespace PhoneModules
{
    [DataContract]
    //[MayhemModule("WP7 Reaction", "Triggers from WP7 Mayhem")]
    public class ReactionFromWP7 : EventBase, IWpfConfigurable
    {
        PhoneConnector phoneConnector = PhoneConnector.Instance;

        [DataMember]
        private string id = "";

        protected override void Initialize()
        {
            id = Guid.NewGuid().ToString();
        }

        void phoneConnector_EventCalled(string eventText)
        {
            if (eventText == id)
            {
                base.Trigger();
            }
        }

        public override bool Enable()
        {
            phoneConnector.Enable();
            phoneConnector.EventCalled += phoneConnector_EventCalled;

            return true;
        }

        public override void Disable()
        {
            phoneConnector.Disable();
            phoneConnector.EventCalled -= phoneConnector_EventCalled;
            base.Disable();
        }

        IWpfConfiguration IWpfConfigurable.ConfigurationControl
        {
            get
            {
                return new ReactionFromWP7Config(id);
            }
        }

        public void OnSaved(IWpfConfiguration configurationControl)
        {
            
        }
    }
}
