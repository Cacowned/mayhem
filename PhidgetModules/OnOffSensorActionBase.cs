using System;
using System.Runtime.Serialization;
using Phidgets.Events;

namespace PhidgetModules
{
	[Serializable]
	abstract public class OnOffSensorActionBase : SensorActionBase, ISerializable
	{
		protected int topThreshold = 900;
		protected int bottomThreshold = 500;

		protected double value;
		protected double lastValue;

		// If this is true, then we want to trigger
		// when this sensor turns "on" otherwise
		// trigger when this sensor turns "off"
		protected bool onTurnOn = true;

		public OnOffSensorActionBase(string name, string description)
			: base(name, description) {
			Setup();
		}

		protected override void Setup() {
			base.Setup();

			hasConfig = true;
			value = lastValue = 0;

			SetConfigString();
		}

		protected abstract void SetConfigString();

		protected override void SensorChange(object sender, SensorChangeEventArgs e) {
			// We only care about the index we are watching
			if (e.Index != index)
				return;

			value = e.Value;

			if (onTurnOn && value >= topThreshold && lastValue < topThreshold) {
				OnActionActivated();
			} else if (!onTurnOn && value <= bottomThreshold && lastValue > bottomThreshold) {
				OnActionActivated();
			}

			lastValue = value;
		}

		#region Serialization

		public OnOffSensorActionBase(SerializationInfo info, StreamingContext context)
			: base(info, context) {
			topThreshold = info.GetInt32("TopThreshold");
			bottomThreshold = info.GetInt32("BottomThreshold");

			onTurnOn = info.GetBoolean("OnTurnOn");

			Setup();
		}

		public new void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);

			info.AddValue("TopThreshold", topThreshold);
			info.AddValue("BottomThreshold", bottomThreshold);

			info.AddValue("OnTurnOn", onTurnOn);
		}
		#endregion
	}
}
