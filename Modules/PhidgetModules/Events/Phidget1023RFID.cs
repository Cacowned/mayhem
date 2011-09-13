using System;
using System.Runtime.Serialization;
using System.Windows.Controls;
using MayhemCore;
using MayhemCore.ModuleTypes;
using MayhemDefaultStyles.UserControls;
using PhidgetModules.Wpf;
using Phidgets;
using Phidgets.Events;

namespace PhidgetModules.Events
{
    [DataContract]
    [MayhemModule("Phidget: RFID", "Triggers with a certain RFID Tag")]
    public class Phidget1023RFID : EventBase, IWpfConfigurable
    {
        #region Configuration
        // This is the tag we are watching for
        [DataMember]
        private string Tag
        {
            get;
            set;
        }

        #endregion

        protected RFID rfid;

        protected override void Initialize()
        {
            base.Initialize();

            rfid = InterfaceFactory.GetRFID();
        }

        public IWpfConfiguration ConfigurationControl
        {
            get { return new Phidget1023RFIDConfig(Tag); }
        }

        public void OnSaved(IWpfConfiguration configurationControl)
        {
            Tag = ((Phidget1023RFIDConfig)configurationControl).TagID;
        }

        public override void SetConfigString()
        {
            ConfigString = String.Format("RFID Tag ID {0}", Tag);
        }


        //Tag event handler...we'll display the tag code in the field on the GUI
        void RfidTag(object sender, TagEventArgs e)
        {
            if (e.Tag == Tag)
            {
                OnEventActivated();
                rfid.LED = true;
            }
        }

        //Tag event handler...we'll display the tag code in the field on the GUI
        void LostRfidTag(object sender, TagEventArgs e)
        {
            rfid.LED = false;
        }

        public override void Enable()
        {
            base.Enable();
            rfid.Tag += RfidTag;
            rfid.TagLost += LostRfidTag;
        }

        public override void Disable()
        {
            base.Disable();

            if (rfid != null)
            {
                rfid.Tag -= RfidTag;
                rfid.TagLost -= LostRfidTag;
            }

        }
    }
}
