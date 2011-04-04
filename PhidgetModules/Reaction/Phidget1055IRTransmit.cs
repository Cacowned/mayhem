using System;
using System.Runtime.Serialization;
using System.Windows;
using MayhemCore;
using MayhemCore.ModuleTypes;
using Phidgets;

namespace PhidgetModules.Reaction
{
    [Serializable]
    public class Phidget1055IRTransmit : ReactionBase, IWpf, ISerializable
    {
        protected IR ir;

        // This is the code and code information that we will
        // transmit
        protected IRCode code;
        protected IRCodeInfo codeInfo;
        
        public Phidget1055IRTransmit()
            : base("Phidget-1055: IR Transmit", "Sends an IR code")
        {
            Setup();
        }

        protected virtual void Setup()
        {
            hasConfig = true;

            ir = InterfaceFactory.GetIR();

            SetConfigString();
        }

        public void WpfConfig()
        {

            var window = new Phidget1055IRTransmitConfig(code, codeInfo);
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            window.ShowDialog();

            if (window.DialogResult == true)
            {
                code = window.Code;
                codeInfo = window.CodeInfo;

                SetConfigString();
            }
        }

        protected void SetConfigString()
        {
            ConfigString = String.Format("IR Code 0x{0}", code);
        }

        public override void Perform()
        {
            if (code != null && codeInfo != null)
            {
                ir.transmit(code, codeInfo);
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

        #region Serialization

        public Phidget1055IRTransmit(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            code = (IRCode)info.GetValue("Code", typeof(IRCode));
            codeInfo = (IRCodeInfo)info.GetValue("CodeInfo", typeof(IRCodeInfo));
            Setup();

        }

        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("Code", code);
            info.AddValue("CodeInfo", codeInfo);
        }
        #endregion
    }
}
