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
	public class Phidget1101IRDistance : RangeSensorActionBase, IWpf, ISerializable
	{

		public Phidget1101IRDistance()
			: base("Phidget-1101: IR Distance", "Triggers at a certain distance") {

			Setup();
		}

		public void WpfConfig() {
			var window = new Phidget1101IRDistanceConfig(ifKit, index, topValue, bottomValue, ConvertToString);
			window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

			if (window.ShowDialog() == true) {
				index = window.index;
				topValue = window.topValue;
				bottomValue = window.bottomValue;

				SetConfigString();
			}
		}

		protected override bool IsValidInput(int value) {
			return (value < 490 && value > 80);
		}

		public override double Convert(int value) {
			return 9462 / (value - 16.92);
		}

		public string ConvertToString(int value) {

            if ((value < 490) && (value > 80))
            {
                return Convert(value).ToString("0.##") + " cm";
            }
            else
            {
                return "Object Not Detected";
            }
		}

		protected override void SetConfigString() {
			ConfigString = String.Format("Index {0}, between {1} and {2} cm", index, topValue.ToString("0.##"), bottomValue.ToString("0.##"));
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
