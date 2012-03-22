using System;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using PhidgetModules.Wpf;
using Phidgets;
using Phidgets.Events;
using PhidgetModules.Wpf.UserControls;

namespace PhidgetModules.Events
{
	[DataContract]
	[MayhemModule("Phidget: Digital Input", "Triggers on a digital input")]
	public class DigitalInput : EventBase, IWpfConfigurable
	{
		// Which index do we want to be looking at?
		[DataMember]
		private int index;

		// Toggle when it goes on, or when it goes off?
		[DataMember]
		private bool onWhenOn;

		// The interface kit we are using for the sensors
		private InterfaceKit ifKit;
		
		protected override void OnLoadDefaults()
		{
			index = 0;
			onWhenOn = true;
		}

		public WpfConfiguration ConfigurationControl
		{
			get
			{
				return new SensorConfig(index, ConvertToString, new ConfigDigitalInput(onWhenOn), InterfaceKitType.Input);
			}
		}

		public string ConvertToString(int value)
		{
			if (value != 0)
				return "Pressed";

			return "Released";
		}

		public void OnSaved(WpfConfiguration configurationControl)
		{
			SensorConfig sensor = configurationControl as SensorConfig;
			ConfigDigitalInput config = sensor.Sensor as ConfigDigitalInput;

			index = sensor.Index;
			onWhenOn = config.OnWhenOn;
		}

		public string GetConfigString()
		{
			string type = "pressed";
			if (!onWhenOn)
			{
				type = "released";
			}

			return string.Format("Triggers when input #{0} {1}", index, type);
		}

		// The input has changed, do the work here
		private void InputChange(object sender, InputChangeEventArgs ex)
		{
			// If e.CurrentValue is true, then it used to be false.
			// Trigger when appropriate

			// We are dealing with the right input
			if (ex.Index == index)
			{
				// If its true and we turn on when it turns on
				// then trigger
				if (ex.Value && onWhenOn)
				{
					Trigger();
				}
				else if (!ex.Value && !onWhenOn)
				{
					// otherwise, if it its off, and we trigger
					// when it turns off, then trigger
					Trigger();
				}
			}
		}

		protected override void OnEnabling(EnablingEventArgs e)
		{
			if (!e.WasConfiguring)
			{
				try
				{
					ifKit = PhidgetManager.Get<InterfaceKit>();
				}
				catch (InvalidOperationException)
				{
					ErrorLog.AddError(ErrorType.Failure, "The Phidget interface kit is not attached");
					e.Cancel = true;
					return;
				}
			}
			ifKit.InputChange += InputChange;
		}

		protected override void OnDisabled(DisabledEventArgs e)
		{
			ifKit.InputChange -= InputChange;
			if (!e.IsConfiguring)
			{
				PhidgetManager.Release<InterfaceKit>(ref ifKit);
			}
		}
	}
}
