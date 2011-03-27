using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using MayhemCore.ModuleTypes;
using System.Runtime.Serialization;
using Phidgets;
using Phidgets.Events;
using PhidgetModules.Wpf;
using System.Windows;

namespace PhidgetModules.Action
{
    [Serializable]
    public class Phidget1055IRReceive : ActionBase, IWpf, ISerializable
    {
        protected IR ir;

        protected IRCodeEventHandler gotCode;

        protected IRCode code;

        protected DateTime lastSignal;

        // This is the tag we are watching for
        protected string ourTag = String.Empty;

        public Phidget1055IRReceive()
            : base("Phidget-1055: IR Receive", "Triggers when it sees a certain IR code")
        {
            Setup();
        }

        protected virtual void Setup()
        {
            hasConfig = true;

            ir = InterfaceFactory.GetIR();

            gotCode = new IRCodeEventHandler(ir_Code);
            SetConfigString();
        }

        public void WpfConfig()
        {

            var window = new Phidget1055IRReceiveConfig(code);
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            window.ShowDialog();

            if (window.DialogResult == true)
            {
                code = window.Code;

                SetConfigString();
            }
        }

        protected void SetConfigString()
        {
            ConfigString = String.Format("IR Code 0x{0}", code);
        }

        // When we receive a code
        void ir_Code(object sender, IRCodeEventArgs e)
        {
            // If the data matches,
            // Do we care about the number of times it was repeated?
            if (code.Data.SequenceEqual(e.Code.Data))
            {
                // We need to make a timeout for the IR
                TimeSpan diff = DateTime.Now - lastSignal;
                if (diff.TotalMilliseconds >= 750)
                {

                    // then trigger
                    OnActionActivated();
                }

                lastSignal = DateTime.Now;
            }
        }

        public override void Enable()
        {
            base.Enable();
            ir.Code += gotCode;
        }

        public override void Disable()
        {
            base.Disable();

            if (ir != null)
            {
                ir.Code -= gotCode;
            }
        }

        #region Serialization

        public Phidget1055IRReceive(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            code = (IRCode)info.GetValue("Code", typeof(IRCode));
            Setup();

        }

        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("Code", code);
        }
        #endregion
    }
}
