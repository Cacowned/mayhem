using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using MayhemCore.ModuleTypes;
using System.Windows;
using PhidgetModules.Wpf;

namespace PhidgetModules.Action
{
	[Serializable]
	public class Phidget1127Light : ValueSensorActionBase, IWpf, ISerializable
	{

		public Phidget1127Light()
			: base("Phidget-1127: Light Sensor", "Triggers at a certain light level") {
			Setup();
		}

		public void WpfConfig() {
			var window = new Phidget1127LightConfig(ifKit, index, topValue, increasing);
			window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

			if (window.ShowDialog() == true) {
				index = window.index;
				topValue = window.topValue;
				increasing = window.increasing;

				SetConfigString();
			}
		}

		public override double Convert(int value) {
			return (double) value;
		}

		protected override void SetConfigString() {
			string overUnder = "above";
			if (!increasing) {
				overUnder = "below";
			}

			ConfigString = String.Format("Index {0} goes {1} {2} lx", index, overUnder, topValue.ToString("0.###"));
		}


		#region Serialization

		public Phidget1127Light(SerializationInfo info, StreamingContext context)
			: base(info, context) {

		}

		public new void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);

		}
		#endregion
	}
}
