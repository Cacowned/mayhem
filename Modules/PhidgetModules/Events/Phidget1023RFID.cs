using System.Runtime.Serialization;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using PhidgetModules.Wpf;
using Phidgets;
using Phidgets.Events;

namespace PhidgetModules.Events
{
    [DataContract]
    [MayhemModule("Phidget: Rfid", "Triggers with a certain Rfid Tag")]
    internal class Phidget1023Rfid : EventBase, IWpfConfigurable
    {
        // This is the tag we are watching for
        [DataMember]
        private string tag;

        private RFID rfid;

        protected override void OnAfterLoad()
        {
            rfid = InterfaceFactory.Rfid;
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new Phidget1023RFIDConfig(tag); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            tag = ((Phidget1023RFIDConfig)configurationControl).TagID;
        }

        public string GetConfigString()
        {
            return string.Format("Rfid Tag ID {0}", tag);
        }

        // Tag event handler...we'll display the tag code in the field on the GUI
        private void RfidTag(object sender, TagEventArgs e)
        {
            if (e.Tag == tag)
            {
                Trigger();
                rfid.LED = true;
            }
        }

        // Tag event handler...we'll display the tag code in the field on the GUI
        private void LostRfidTag(object sender, TagEventArgs e)
        {
            rfid.LED = false;
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
            rfid.Tag += RfidTag;
            rfid.TagLost += LostRfidTag;
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            if (rfid != null)
            {
                rfid.Tag -= RfidTag;
                rfid.TagLost -= LostRfidTag;
            }
        }
    }
}
