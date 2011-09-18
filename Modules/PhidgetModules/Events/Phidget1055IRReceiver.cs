using System;
using System.Linq;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemCore.ModuleTypes;
using MayhemWpf.UserControls;
using PhidgetModules.Wpf;
using Phidgets;
using Phidgets.Events;

namespace PhidgetModules.Events
{
    [DataContract]
    [MayhemModule("Phidget: IR Receiver", "Triggers when it sees a certain IR code")]
    public class Phidget1055IRReceiver : EventBase, IWpfConfigurable
    {
        #region Configuration

        [DataMember]
        private IRCode Code
        {
            get;
            set;
        }
        #endregion

        private IR ir;
        private IRCodeEventHandler gotCode;
        private DateTime lastSignal;

        protected override void Initialize()
        {
            base.Initialize();

            ir = InterfaceFactory.IR;

            gotCode = new IRCodeEventHandler(ir_Code);
        }

        public IWpfConfiguration ConfigurationControl
        {
            get { return new Phidget1055IRReceiveConfig(Code); }
        }

        public void OnSaved(IWpfConfiguration configurationControl)
        {
            Code = ((Phidget1055IRReceiveConfig)configurationControl).Code;
        }

        public override void SetConfigString()
        {
            ConfigString = String.Format("IR Code 0x{0}", Code);
        }

        // When we receive a code
        private void ir_Code(object sender, IRCodeEventArgs e)
        {
            // If the data matches,
            // Do we care about the number of times it was repeated?
            if (Code.Data.SequenceEqual(e.Code.Data))
            {
                // We need to make a timeout for the IR
                TimeSpan diff = DateTime.Now - lastSignal;
                if (diff.TotalMilliseconds >= 750)
                {

                    // then trigger
                    Trigger();
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
    }
}
