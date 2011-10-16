using System;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using PhidgetModules.Wpf;
using PhidgetModules.Wpf.UserControls;

namespace PhidgetModules.Events
{
    [DataContract]
    [MayhemModule("Phidget: Force Sensor", "Triggers at a certain force level")]
    public class Phidget1106Force : ValueSensorEventBase, IWpfConfigurable
    {
        public WpfConfiguration ConfigurationControl
        {
            get { return new SensorConfig(IfKit, Index, ConvertToString, new Config1106Force(TopValue, Increasing)); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            SensorConfig sensor = configurationControl as SensorConfig;
            Config1106Force config = sensor.Sensor as Config1106Force;

            Index = sensor.Index;
            TopValue = config.TopValue;
            Increasing = config.Increasing;
        }

        public override double Convert(int value)
        {
            return (double)value;
        }

        protected string ConvertToString(int value)
        {
            return value.ToString("0.###");
        }

        public string GetConfigString()
        {
            string overUnder = "goes above";
            if (!Increasing)
            {
                overUnder = "drops below";
            }

            return string.Format("Index {0} {1} {2}", Index, overUnder, TopValue.ToString("0.###"));
        }
    }
}
