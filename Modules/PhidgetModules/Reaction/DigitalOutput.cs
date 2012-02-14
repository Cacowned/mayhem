using System.Runtime.Serialization;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using PhidgetModules.Wpf;
using Phidgets;

namespace PhidgetModules.Reaction
{
	[DataContract]
	[MayhemModule("Phidget: Digital Output", "Triggers a digital output")]
	public class DigitalOutput : ReactionBase, IWpfConfigurable
	{
		// Which index do we want to be looking at?
		[DataMember]
		private int index;

		[DataMember]
		private DigitalOutputType outputType;

		// The interface kit we are using for the sensors
		private InterfaceKit ifKit;

		protected override void OnLoadDefaults()
		{
			index = 0;
			outputType = DigitalOutputType.Toggle;
		}

		protected override void OnAfterLoad()
		{
			ifKit = InterfaceFactory.Interface;
		}

		public WpfConfiguration ConfigurationControl
		{
			get { return new PhidgetDigitalOutputConfig(ifKit, index, outputType); }
		}

		public void OnSaved(WpfConfiguration configurationControl)
		{
			index = ((PhidgetDigitalOutputConfig)configurationControl).Index;
			outputType = ((PhidgetDigitalOutputConfig)configurationControl).OutputType;
		}

		public string GetConfigString()
		{
			string type = string.Empty;

			switch (outputType)
			{
				case DigitalOutputType.Toggle: type = "Toggle";
					break;
				case DigitalOutputType.On: type = "Turn On";
					break;
				case DigitalOutputType.Off: type = "Turn Off";
					break;
			}

			return type + " output #" + index;
		}

		public override void Perform()
		{
			if (ifKit.Attached)
			{
				switch (outputType)
				{
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
			}
			else
			{
				ErrorLog.AddError(ErrorType.Failure, "Phidget Interface Kit is not attached");
			}
		}
	}

	// What action we should take on the digital output
	public enum DigitalOutputType
	{
		On,
		Off,
		Toggle
	}
}
