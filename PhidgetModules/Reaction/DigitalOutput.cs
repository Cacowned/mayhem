using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using System.Runtime.Serialization;
using Phidgets;

namespace PhidgetModules.Reaction
{
	[Serializable]
	public class DigitalOutput : ReactionBase, ISerializable
	{
		// Which index do we want to be looking at?
		protected int index;

		// The interface kit we are using for the sensors
		protected InterfaceKit ifKit;

		protected bool flag = true;

		public DigitalOutput()
			: base("Phidget: Digital Output", "Triggers a digital output") {
				
			index = 0;

			Setup();
		}

		protected void Setup() {
			this.ifKit = InterfaceFactory.GetInterface();

		}

		public override void Perform() {
			this.ifKit.outputs[index] = flag;
			flag = !flag;
		}

		#region Serialization
        public DigitalOutput(SerializationInfo info, StreamingContext context) 
            : base (info, context)
        {
            Setup();
        }

        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
        #endregion
	}
}
