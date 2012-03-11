using System.Runtime.Serialization;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using PhidgetModules.Wpf;
using PhidgetModules.Wpf.UserControls;

namespace PhidgetModules.Events
{
	[DataContract]
	[MayhemModule("Phidget: Proximity Sensor", "Triggers based on object detection")]
	public class Phidget1103IRReflective : OnOffSensorEventBase, IWpfConfigurable
	{
		protected override void OnLoadDefaults()
		{
            base.OnLoadDefaults();

			BottomThreshold = 0;
			TopThreshold = 100;
		}

        protected override void OnBeforeLoad()
        {
            base.OnBeforeLoad();

            // We invert because the proximity sensor actually says that it sees an object
            // when the value drops below 100 opposed to sensors like touch which shoot up when they
            // detect.
            base.invert = true;
        }

		public WpfConfiguration ConfigurationControl
		{
			get { return new SensorConfig(Index, ConvertToString, new Config1103IrReflective(OnTurnOn)); }
		}

		public void OnSaved(WpfConfiguration configurationControl)
		{
			SensorConfig sensor = configurationControl as SensorConfig;
			Config1103IrReflective config = sensor.Sensor as Config1103IrReflective;

			Index = sensor.Index;

			// The reason we negate here is explained above.
			OnTurnOn = config.OnTurnOn;
		}

		public string GetConfigString()
		{
			string message = "recognizes an object";
			if (!OnTurnOn)
			{
				message = "stops recognizing an object";
			}

			return string.Format("Index {0} {1}", Index, message);
		}

		protected string ConvertToString(int value)
		{
			if (value < 100)
				return "Detected";

			return "Not Detected";
		}
	}
}
