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
    [MayhemModule("Phidget: Sound Sensor", "Triggers at a certain decibel level")]
    public class Phidget1133Sound : ValueSensorEventBase, IWpfConfigurable
    {
        public IWpfConfiguration ConfigurationControl
        {
            get { return new SensorConfig(IfKit, Index, ConvertToString, new Config1133Sound(TopValue, Increasing)); }
        }

        public void OnSaved(IWpfConfiguration configurationControl)
        {
            SensorConfig sensor = configurationControl as SensorConfig;
            Config1133Sound config = sensor.Sensor as Config1133Sound;

            Index = sensor.Index;
            TopValue = config.TopValue;
            Increasing = config.Increasing;
        }

        public override double Convert(int value)
        {
            return 16.801 * Math.Log(value) + 9.872;
        }

        public string ConvertToString(int value)
        {
            return Convert(value).ToString("0.###") + " db";
        }

        public override void SetConfigString()
        {
            string overUnder = "above";
            if (!Increasing)
            {
                overUnder = "below";
            }

            ConfigString = String.Format("Index {0} goes {1} {2} dB", Index, overUnder, TopValue.ToString("0.###"));
        }


    }
}
