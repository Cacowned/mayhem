using System.Runtime.Serialization;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using PhidgetModules.Wpf;
using PhidgetModules.Wpf.UserControls;

namespace PhidgetModules.Events
{
	[DataContract]
	[MayhemModule("Phidget: IR Distance", "Triggers at a certain distance")]
	public class Phidget1101IrDistance : RangeSensorEventBase, IWpfConfigurable
	{
		public WpfConfiguration ConfigurationControl
		{
			get { return new SensorConfig(Index, ConvertToString, new Config1101IrDistance(TopValue, BottomValue)); }
		}

		public void OnSaved(WpfConfiguration configurationControl)
		{
			SensorConfig sensor = configurationControl as SensorConfig;
			Config1101IrDistance config = sensor.Sensor as Config1101IrDistance;

			Index = sensor.Index;
			TopValue = config.TopValue;
			BottomValue = config.BottomValue;
		}

		protected override bool IsValidInput(int value)
		{
			return value < 490 && value > 80;
		}

		public override double Convert(int value)
		{
			return 9462 / (value - 16.92);
		}

		public string ConvertToString(int value)
		{
			if ((value < 490) && (value > 80))
			{
				return Convert(value).ToString("0.##") + " cm";
			}

			return "Object Not Detected";
		}

		public string GetConfigString()
		{
			return string.Format("Index {0}, between {1} and {2} cm", Index, TopValue.ToString("0.##"), BottomValue.ToString("0.##"));
		}
	}
}
