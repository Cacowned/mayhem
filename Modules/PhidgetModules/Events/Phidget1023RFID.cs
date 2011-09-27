using System;
using System.Runtime.Serialization;
using System.Windows.Controls;
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
    public class Phidget1023Rfid : EventBase, IWpfConfigurable
    {
        #region Configuration
        // This is the tag we are watching for
        [DataMember]
        private string Tag;

        #endregion

        private RFID rfid;

        protected override void Initialize()
        {
            rfid = InterfaceFactory.Rfid;
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new Phidget1023RFIDConfig(Tag); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            Tag = ((Phidget1023RFIDConfig)configurationControl).TagID;
        }

        public string GetConfigString()
        {
            return String.Format("Rfid Tag ID {0}", Tag);
        }

        //Tag event handler...we'll display the tag code in the field on the GUI
        private void RfidTag(object sender, TagEventArgs e)
        {
            if (e.Tag == Tag)
            {
                Trigger();
                rfid.LED = true;
            }
        }

        //Tag event handler...we'll display the tag code in the field on the GUI
        void LostRfidTag(object sender, TagEventArgs e)
        {
            rfid.LED = false;
        }

        public override bool Enable()
        {
            rfid.Tag += RfidTag;
            rfid.TagLost += LostRfidTag;

            return true;
        }

        public override void Disable()
        {
            if (rfid != null)
            {
                rfid.Tag -= RfidTag;
                rfid.TagLost -= LostRfidTag;
            }

        }
    }
}
