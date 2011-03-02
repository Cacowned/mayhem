﻿using System;
using System.Runtime.Serialization;
using System.Windows;
using MayhemCore.ModuleTypes;
using PhidgetModules.Wpf;

namespace PhidgetModules.Action
{
	[Serializable]
	public class Phidget1129Touch : OnOffSensorActionBase, IWpf, ISerializable
	{

		public Phidget1129Touch()
			: base("Phidget-1129: Touch Sensor", "Triggers based on touching the sensor") {
			Setup();
		}

		protected override void Setup() {
			hasConfig = true;
			base.Setup();
		}

		public void WpfConfig() {
			var window = new Phidget1129TouchConfig(ifKit, index, onTurnOn, ConvertToString);
			window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

			if (window.ShowDialog() == true) {
				index = window.index;
				onTurnOn = window.onTurnOn;

				SetConfigString();
			}
		}

		public string ConvertToString(int value) {
			if (value > 500)
				return "Touch Detected";
			else
				return "No Touch Detected";
		}

		protected override void SetConfigString() {
			string message = "turns on";
			if (!onTurnOn) {
				message = "turns off";
			}

			ConfigString = String.Format("Index {0} {1}", index, message);
		}


		#region Serialization

		public Phidget1129Touch(SerializationInfo info, StreamingContext context)
			: base(info, context) {

		}

		public new void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);

		}
		#endregion


	}
}
