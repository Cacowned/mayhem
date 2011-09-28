using System;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using Phidgets;

namespace PhidgetModules.Reaction
{
    [DataContract]
    [MayhemModule("Phidget: IR Transmit", "Sends an IR code")]
    public class Phidget1055IRTransmit : ReactionBase, IWpfConfigurable
    {
        #region Configuration

        // This is the code and code information that we will
        // transmit
        [DataMember]
        private IRCode Code;

        [DataMember]
        private IRCodeInfo CodeInfo;
        #endregion

        private IR ir;

        protected override void Initialize()
        {
            ir = InterfaceFactory.IR;
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new Phidget1055IRTransmitConfig(Code, CodeInfo); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            Code = ((Phidget1055IRTransmitConfig)configurationControl).Code;
            CodeInfo = ((Phidget1055IRTransmitConfig)configurationControl).CodeInfo;
        }

        public string GetConfigString()
        {
            return String.Format("IR Code 0x{0}", Code);
        }

        public override void Perform()
        {
            if (Code != null && CodeInfo != null)
            {
                ir.transmit(Code, CodeInfo);
                ir.transmitRepeat();
            }
        }
    }
}
