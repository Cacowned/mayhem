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
    [MayhemModule("Phidget: Light Sensor", "Triggers at a certain light level")]
    public class Phidget1127Light : ValueSensorEventBase, IWpfConfigurable
    {
        public IWpfConfiguration ConfigurationControl
        {
            get { return new SensorConfig(IfKit, Index, ConvertToString, new Config1127Light(TopValue, Increasing)); }
        }

        public void OnSaved(IWpfConfiguration configurationControl)
        {
            SensorConfig sensor = configurationControl as SensorConfig;
            Config1127Light config = sensor.Sensor as Config1127Light;

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
            return value.ToString("0.###") + " lx";
        }

        public override void SetConfigString()
        {
            string overUnder = "above";
            if (!Increasing)
            {
                overUnder = "below";
            }

            ConfigString = String.Format("Index {0} goes {1} {2} lx", Index, overUnder, TopValue.ToString("0.###"));
        }
    }
}
