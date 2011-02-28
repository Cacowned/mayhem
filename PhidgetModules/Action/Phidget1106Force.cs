using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore.ModuleTypes;
using System.Runtime.Serialization;
using System.Windows;
using PhidgetModules.Wpf;

namespace PhidgetModules.Action
{
	[Serializable]
	public class Phidget1106Force : ValueSensorActionBase, IWpf, ISerializable
	{

		public Phidget1106Force()
			: base("Phidget-1106: Force Sensor", "Triggers at a certain force level") {
			Setup();
		}

		public void WpfConfig() {
			var window = new Phidget1106ForceConfig(ifKit, index, topValue, increasing, ConvertToString);
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
		
		protected string ConvertToString(int value) {
			return value.ToString("0.###");
		} 

		protected override void SetConfigString() {
			string overUnder = "above";
			if (!increasing) {
				overUnder = "below";
			}

			ConfigString = String.Format("Index {0} goes {1} {2}", index, overUnder, topValue.ToString("0.###"));
		}


		#region Serialization

		public Phidget1106Force(SerializationInfo info, StreamingContext context)
			: base(info, context) {

		}

		public new void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);

		}
		#endregion
	}
}
