using System.Runtime.Serialization;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using PhidgetModules.Wpf;
using Phidgets;
using System;

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

		public WpfConfiguration ConfigurationControl
		{
			get { return new PhidgetDigitalOutputConfig(ifKit, index, outputType); }
		}

		public void OnSaved(WpfConfiguration configurationControl)
		{
			index = ((PhidgetDigitalOutputConfig)configurationControl).Index;
			outputType = ((PhidgetDigitalOutputConfig)configurationControl).OutputType;
		}

		protected override void OnEnabling(EnablingEventArgs e)
		{
			try
			{
				ifKit = PhidgetManager.Get<InterfaceKit>();
			}
			catch (InvalidOperationException)
			{
				ErrorLog.AddError(ErrorType.Failure, "The interface kit is not attached");
				e.Cancel = true;
				return;
			}
		}

		protected override void OnDisabled(DisabledEventArgs e)
		{
			PhidgetManager.Release<InterfaceKit>(ref ifKit);
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
