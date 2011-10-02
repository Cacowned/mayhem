using System;
using System.Linq;
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
    [MayhemModule("Phidget: IR Receiver", "Triggers when it sees a certain IR code")]
    public class Phidget1055IRReceiver : EventBase, IWpfConfigurable
    {
        [DataMember]
        private IRCode Code;

        private IR ir;
        private IRCodeEventHandler gotCode;
        private DateTime lastSignal;

        protected override void OnAfterLoad()
        {
            ir = InterfaceFactory.IR;

            gotCode = new IRCodeEventHandler(ir_Code);
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new Phidget1055IRReceiveConfig(Code); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            Code = ((Phidget1055IRReceiveConfig)configurationControl).Code;
        }

        public string GetConfigString()
        {
            return String.Format("IR Code 0x{0}", Code);
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

        protected override void OnEnabling(EnablingEventArgs e)
        {
            ir.Code += gotCode;
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            if (ir != null)
            {
                ir.Code -= gotCode;
            }
        }
    }
}
