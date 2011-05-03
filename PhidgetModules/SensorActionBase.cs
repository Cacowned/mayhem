using System;
using System.Runtime.Serialization;
using MayhemCore;
using Phidgets;
using Phidgets.Events;

namespace PhidgetModules
{
	[Serializable]
	abstract public class SensorActionBase : ActionBase, ISerializable
	{
		// Which index do we want to be looking at?
		protected int index;

		// The interface kit we are using for the sensors
		protected static InterfaceKit ifKit;

		protected SensorChangeEventHandler handler;

		public SensorActionBase(string name, string description)
			: base(name, description) {
			// Default to first index
			index = 0;
			Setup();
		}

		protected virtual void Setup() {
			// If we don't have an ifKit yet, create one
			if (ifKit == null) {
				ifKit = InterfaceFactory.GetInterface();
			}

			handler = new SensorChangeEventHandler(SensorChange);

		}

		protected abstract void SensorChange(object sender, SensorChangeEventArgs e);

		public override void Enable() {
            if (!ifKit.Attached)
            {
                ErrorLog.AddError(ErrorType.Warning, "No Phidget Interface Kit found. Not Enabling.");
            }
            else
            {
                base.Enable();
                ifKit.SensorChange += handler;
            }
		}

		public override void Disable() {
			base.Disable();
			
			if (ifKit != null) {
				ifKit.SensorChange -= handler;
			}

		}

		#region Serialization

		public SensorActionBase(SerializationInfo info, StreamingContext context)
			: base(info, context) {
			index = info.GetInt32("Index");

			Setup();

		}

		public new void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);
			info.AddValue("Index", index);
			// Save the index
		}
		#endregion
	}
}
