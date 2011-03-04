using System;
using System.Runtime.Serialization;
using System.Windows;
using MayhemCore;
using MayhemCore.ModuleTypes;
using PhidgetModules.Wpf;
using Phidgets;

namespace PhidgetModules.Reaction
{
	[Serializable]
	public class DigitalOutput : ReactionBase, IWpf, ISerializable
	{
		// Which index do we want to be looking at?
		protected int index;

		// The interface kit we are using for the sensors
		protected InterfaceKit ifKit;

		protected DigitalOutputType outputType;

		public DigitalOutput()
			: base("Phidget: Digital Output", "Triggers a digital output") {

			index = 0;
			Setup();
		}

		protected void Setup() {
			hasConfig = true;
			this.ifKit = InterfaceFactory.GetInterface();

			SetConfigString();
		}

		public void WpfConfig() {
			var window = new PhidgetDigitalOutputConfig(ifKit, index, outputType: outputType);
			window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

			window.ShowDialog();

			if (window.DialogResult == true) {
				this.index = window.Index;

				this.outputType = window.OutputType;

				SetConfigString();
			}
		}

		public void SetConfigString() {
			string type = "";

			switch (outputType) {
				case DigitalOutputType.Toggle: type = "Toggle";
					break;
				case DigitalOutputType.On: type = "Turn On";
					break;
				case DigitalOutputType.Off: type = "Turn Off";
					break;
			}

			ConfigString = type + " output #" + index;
		}

		public override void Perform() {

			switch (outputType) {
				case DigitalOutputType.Toggle:
					ifKit.outputs[index] = !ifKit.outputs[index];
					break;
				case DigitalOutputType.On:
					ifKit.outputs[index] = true;
					break;
				case DigitalOutputType.Off:
					ifKit.outputs[index] = false;
					break;
			}

			/*
			this.ifKit.outputs[index] = flag;
			flag = !flag;
			 */
		}

		#region Serialization
		public DigitalOutput(SerializationInfo info, StreamingContext context)
			: base(info, context) {

				outputType = (DigitalOutputType)info.GetValue("OutputType", typeof(DigitalOutputType));
				index = info.GetInt32("Index");

			Setup();
		}

		public new void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);

			info.AddValue("OutputType", outputType);
			info.AddValue("Index", index);
		}
		#endregion
	}

	// What action we should take on the digital output
	public enum DigitalOutputType
	{
		On,
		Off,
		Toggle
	}
}
