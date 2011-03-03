using System;
using System.Runtime.Serialization;
using System.Windows;
using MayhemCore.ModuleTypes;
using PhidgetModules.Wpf;

namespace PhidgetModules.Action
{
	[Serializable]
	public class Phidget1103IRReflective : OnOffSensorActionBase, IWpf, ISerializable
	{

		public Phidget1103IRReflective()
			: base("Phidget-1103: Reflective IR", "Triggers based on object detection") {
			Setup();
		}

		protected override void Setup() {
			index = 2;
			hasConfig = true;

			// Items are recognized between 0-100 on the data
			bottomThreshold = 0;
			topThreshold = 100;

			base.Setup();
		}

		public void WpfConfig() {
			var window = new Phidget1103IRReflectiveConfig(ifKit, index, onTurnOn, ConvertToString);
			window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

			if (window.ShowDialog() == true) {
				index = window.index;
				onTurnOn = window.onTurnOn;

				SetConfigString();
			}
		}

		protected override void SetConfigString() {
			string message = "recognizes an object";
			if (!onTurnOn) {
				message = "stops recognizing an object";
			}

			ConfigString = String.Format("Index {0} {1}", index, message);
		}

		protected string ConvertToString(int value) {
			if (value < 100)
				return "Detected";
			else
				return "Not Detected";
		}

		#region Serialization

		public Phidget1103IRReflective(SerializationInfo info, StreamingContext context)
			: base(info, context) {

		}

		public new void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);

		}
		#endregion


	}
}
