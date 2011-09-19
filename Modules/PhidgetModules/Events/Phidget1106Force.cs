using System;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemCore.ModuleTypes;
using MayhemWpf.UserControls;
using PhidgetModules.Wpf;
using PhidgetModules.Wpf.UserControls;

namespace PhidgetModules.Events
{
    [DataContract]
    [MayhemModule("Phidget: Force Sensor", "Triggers at a certain force level")]
    public class Phidget1106Force : ValueSensorEventBase, IWpfConfigurable
    {
        public IWpfConfiguration ConfigurationControl
        {
            get { return new SensorConfig(IfKit, Index, ConvertToString, new Config1106Force(TopValue, Increasing)); }
        }

        public void OnSaved(IWpfConfiguration configurationControl)
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

        public override void SetConfigString()
        {
            string overUnder = "above";
            if (!Increasing)
            {
                overUnder = "below";
            }

            ConfigString = String.Format("Index {0} goes {1} {2}", Index, overUnder, TopValue.ToString("0.###"));
        }
    }
}
