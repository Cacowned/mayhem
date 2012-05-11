using System.Runtime.Serialization;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using PhidgetModules.Wpf;
using PhidgetModules.Wpf.UserControls;

namespace PhidgetModules.Events
{
    [DataContract]
    [MayhemModule("Phidget: Light Sensor", "Triggers at a certain light level")]
    public class Phidget1127Light : ValueSensorEventBase, IWpfConfigurable
    {
        public WpfConfiguration ConfigurationControl
        {
            get { return new SensorConfig(Index, ConvertToString, new Config1127Light(TopValue, Increasing)); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            SensorConfig sensor = configurationControl as SensorConfig;
            Config1127Light config = sensor.Sensor as Config1127Light;

            Index = sensor.Index;
            TopValue = config.TopValue;
            Increasing = config.Increasing;
        }

        public override double Convert(int value)
        {
            return value;
        }

        protected string ConvertToString(int value)
        {
            return value.ToString("0.###") + " lx";
        }

        public string GetConfigString()
        {
            string overUnder = "above";
            if (!Increasing)
            {
                overUnder = "below";
            }

            return string.Format("Index {0} goes {1} {2} lx", Index, overUnder, TopValue.ToString("0.###"));
        }
    }
}
