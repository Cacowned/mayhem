using System;
using System.Runtime.Serialization;
using System.Windows;
using MayhemCore;
using MayhemCore.ModuleTypes;
using Phidgets;
using MayhemWpf.UserControls;

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
        private IRCode Code
        {
            get;
            set;
        }
        [DataMember]
        private IRCodeInfo CodeInfo
        {
            get;
            set;
        }
        #endregion

        protected IR ir;

        protected override void Initialize()
        {
            base.Initialize();

            ir = InterfaceFactory.GetIR();
        }

        public IWpfConfiguration ConfigurationControl
        {
            get { return new Phidget1055IRTransmitConfig(Code, CodeInfo); }
        }

        public void OnSaved(IWpfConfiguration configurationControl)
        {
            Code = ((Phidget1055IRTransmitConfig)configurationControl).Code;
            CodeInfo = ((Phidget1055IRTransmitConfig)configurationControl).CodeInfo;
        }

        public override void SetConfigString()
        {
            ConfigString = String.Format("IR Code 0x{0}", Code);
        }

        public override void Perform()
        {
            if (Code != null && CodeInfo != null)
            {
                ir.transmit(Code, CodeInfo);
                ir.transmitRepeat();
            }
        }

        public override void Enable()
        {
            base.Enable();
        }

        public override void Disable()
        {
            base.Disable();
        }


    }
}
