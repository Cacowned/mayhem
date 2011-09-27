using System;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemCore.ModuleTypes;
using MayhemWpf.UserControls;
using PhoneModules.Wpf;

namespace PhoneModules.Events
{
    [DataContract]
    //[MayhemModule("WP7 Reaction", "Triggers from WP7 Mayhem")]
    public class ReactionFromWP7 : EventBase, IWpfConfigurable
    {
        #region Configuration Properties
        [DataMember]
        private string id = "";
        #endregion

        private PhoneConnector phoneConnector = PhoneConnector.Instance;

        protected override void Initialize()
        {
            id = Guid.NewGuid().ToString();
        }

        private void phoneConnector_EventCalled(string eventText)
        {
            if (eventText == id)
            {
                Trigger();
            }
        }

        public override bool Enable()
        {
            phoneConnector.Enable(true);
            phoneConnector.EventCalled += phoneConnector_EventCalled;

            return true;
        }

        public override void Disable()
        {
            phoneConnector.Disable();
            phoneConnector.EventCalled -= phoneConnector_EventCalled;
        }

        #region Configuration Views
        public WpfConfiguration ConfigurationControl
        {
            get
            {
                return new ReactionFromWP7Config(id);
            }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
        }
        #endregion
    }
}
