using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore.ModuleTypes;
using System.Runtime.Serialization;
using PhidgetModules.Wpf;
using System.Windows;

namespace PhidgetModules.Action
{
	[Serializable]
	public class Phidget1101IRDistance : ValueSensorActionBase, IWpf, ISerializable
	{

		public Phidget1101IRDistance()
			: base("Phidget-1101: IR Distance", "Triggers at a certain distance") {

			Setup();
		}

		public void WpfConfig() {
			var window = new Phidget1101IRDistanceConfig(ifKit, index, topValue, increasing, Convert);
			window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

			if (window.ShowDialog() == true) {
				index = window.index;
				topValue = window.topValue;
				increasing = window.increasing;

				SetConfigString();
			}
		}

		protected override bool IsValidRange(int value) {
			return (value < 490 && value > 80);
		}

		public override double Convert(int value) {
			return 9462 / (value - 16.92);
		}

		protected override void SetConfigString() {
			string overUnder = "further";
			if (!increasing) {
				overUnder = "closer";
			}

			ConfigString = String.Format("Index {0}, item {1} than {2} cm", index, overUnder, topValue.ToString("0.##"));
		}


		#region Serialization

		public Phidget1101IRDistance(SerializationInfo info, StreamingContext context)
			: base(info, context) {

		}

		public new void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);

		}
		#endregion
	}
}
