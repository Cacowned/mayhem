using System;
using System.Runtime.Serialization;
using MayhemCore;
using Phidgets;
using Phidgets.Events;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using PhidgetModules.Wpf;

namespace PhidgetModules.Events
{
	[DataContract]
	[MayhemModule("Phidget: Temperature", "Triggers on temperature change")]
	public class Phidget1045Temperature : EventBase, IWpfConfigurable
	{
		private TemperatureSensor sensor;

		[DataMember]
		private bool increasing;

		[DataMember]
		private double topValue;

		private double lastValue;

		protected override void OnLoadDefaults()
		{
			increasing = true;
			topValue = 25;
		}

		protected override void OnAfterLoad()
		{
			lastValue = topValue;
		}

		protected override void OnEnabling(EnablingEventArgs e)
		{
			// If we weren't just configuring, open the sensor
			if (!e.WasConfiguring)
			{
				try
				{
					sensor = PhidgetManager.Get<TemperatureSensor>();
				}
				catch (InvalidOperationException)
				{
					ErrorLog.AddError(ErrorType.Failure, "The Phidget temperature sensor is not attached");
					e.Cancel = true;
					return;
				}
			}

			sensor.TemperatureChange += TemperatureChange;
		}

		protected override void OnDisabled(DisabledEventArgs e)
		{
			sensor.TemperatureChange -= TemperatureChange;
			
			// only release if we aren't going into the configuration menu
			if (!e.IsConfiguring)
			{
				PhidgetManager.Release<TemperatureSensor>(ref sensor);
			}
		}

		private void TemperatureChange(object sender, TemperatureChangeEventArgs e)
		{
			if (increasing && e.Temperature > topValue && lastValue < topValue)
			{
				Trigger();
			}
			else if (!increasing && e.Temperature < topValue && lastValue > topValue)
			{
				Trigger();
			}

			lastValue = e.Temperature;
		}

		public WpfConfiguration ConfigurationControl
		{
			get { return new Phidget1045TemperatureConfig(topValue, increasing); }
		}

		public void OnSaved(WpfConfiguration configurationControl)
		{
			var config = configurationControl as Phidget1045TemperatureConfig;
			topValue = config.TopValue;
			increasing = config.Increasing;
		}

		public string GetConfigString()
		{
			string overUnder = "above";
			if (!increasing)
			{
				overUnder = "below";
			}

			return string.Format("When temp goes {0} {1}", overUnder, topValue.ToString());
		}
	}
}
