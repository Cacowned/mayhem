using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore.ModuleTypes;
using Phidgets.Events;
using System.Runtime.Serialization;
using System.Windows;

namespace PhidgetModules
{
	[Serializable]
	abstract public class ValueSensorActionBase : SensorActionBase, ISerializable
	{
		/// <summary>
		/// If this is true, then we want to trigger when
		/// the value goes from below the topValue to above the
		/// topValue. If it is false, then we trigger when we go
		/// from above to below
		/// </summary>
		protected bool increasing = true;

		protected double topValue = 85;

		protected double value;

		protected double lastValue;

		public ValueSensorActionBase(string name, string description)
			: base(name, description)
		{

			increasing = true;
			topValue = 85;

			Setup();
		}

		protected override void Setup() {
			base.Setup();
			
			hasConfig = true;
			value = lastValue = topValue;

			SetConfigString();
		}

		protected abstract void SetConfigString();

		public abstract double Convert(int value);

		protected virtual bool IsValidRange(int value) {
			return true;
		}

		protected override void SensorChange(object sender, SensorChangeEventArgs e) {
			// We only care about the index we are watching
			if (e.Index != index)
				return;

			if (IsValidRange(e.Value)) {
				value = Convert(e.Value);

				if (increasing && value > topValue && lastValue < topValue) {
					OnActionActivated();
				} else if (!increasing && value < topValue && lastValue > topValue) {
					OnActionActivated();
				}

				lastValue = value;
			}
		}

		#region Serialization

		public ValueSensorActionBase(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			increasing = info.GetBoolean("Increasing");
			topValue = info.GetDouble("TopValue");

			Setup();
		}

		public new void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);

			info.AddValue("Increasing", increasing);
			info.AddValue("TopValue", topValue);
		}
		#endregion
	}
}
