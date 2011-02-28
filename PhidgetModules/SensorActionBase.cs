using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using MayhemCore.ModuleTypes;
using System.Runtime.Serialization;
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
				InterfaceFactory.GetInterface();
			}

			handler = new SensorChangeEventHandler(SensorChange);

		}

		protected abstract void SensorChange(object sender, SensorChangeEventArgs e);

		public override void Enable() {
			base.Enable();
			ifKit.SensorChange += handler;
		}

		public override void Disable() {
			base.Disable();
			ifKit.SensorChange -= handler;
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
