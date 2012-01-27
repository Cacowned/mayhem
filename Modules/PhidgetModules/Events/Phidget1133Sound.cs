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
    [MayhemModule("Phidget: Sound Sensor", "Triggers at a certain decibel level")]
    public class Phidget1133Sound : ValueSensorEventBase, IWpfConfigurable
    {
        public WpfConfiguration ConfigurationControl
        {
            get { return new SensorConfig(IfKit, Index, ConvertToString, new Config1133Sound(TopValue, Increasing)); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            SensorConfig sensor = configurationControl as SensorConfig;
            Config1133Sound config = sensor.Sensor as Config1133Sound;

            Index = sensor.Index;
            TopValue = config.TopValue;
            Increasing = config.Increasing;
        }

        public override double Convert(int value)
        {
            return (16.801 * Math.Log(value)) + 9.872;
        }

        public string ConvertToString(int value)
        {
            return Convert(value).ToString("0.###") + " db";
        }

        public string GetConfigString()
        {
            string overUnder = "above";
            if (!Increasing)
            {
                overUnder = "below";
            }

            return string.Format("Index {0} goes {1} {2} dB", Index, overUnder, TopValue.ToString("0.###"));
        }
    }
}
