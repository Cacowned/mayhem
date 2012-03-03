using System.Runtime.Serialization;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using PhidgetModules.Wpf;
using Phidgets;

namespace PhidgetModules.Reaction
{
    [DataContract]
    [MayhemModule("Phidget: IR Transmit", "Sends an IR code")]
	public class Phidget1055IrTransmit : ReactionBase, IWpfConfigurable
    {
        // This is the code and code information that we will transmit
        //[DataMember]
        private IRCode code;

        //[DataMember]
        private IRCodeInfo codeInfo;

        private IR ir;

        protected override void OnAfterLoad()
        {
            ir = InterfaceFactory.Ir;
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new Phidget1055IrTransmitConfig(code, codeInfo); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            code = ((Phidget1055IrTransmitConfig)configurationControl).Code;
            codeInfo = ((Phidget1055IrTransmitConfig)configurationControl).CodeInfo;
        }

        public string GetConfigString()
        {
            return string.Format("IR Code 0x{0}", code);
        }

        public override void Perform()
        {
            if (code != null && codeInfo != null)
            {
                ir.transmit(code, codeInfo);
                ir.transmitRepeat();
            }
        }
    }
}
