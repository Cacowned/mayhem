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
    [MayhemModule("Phidget: IR Distance", "Triggers at a certain distance")]
    public class Phidget1101IRDistance : RangeSensorEventBase, IWpfConfigurable
    {
        public WpfConfiguration ConfigurationControl
        {
            get { return new SensorConfig(IfKit, Index, ConvertToString, new Config1101IRDistance(TopValue, BottomValue)); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            SensorConfig sensor = configurationControl as SensorConfig;
            Config1101IRDistance config = sensor.Sensor as Config1101IRDistance;

            Index = sensor.Index;
            TopValue = config.TopValue;
            BottomValue = config.BottomValue;
        }

        protected override bool IsValidInput(int value)
        {
            return (value < 490 && value > 80);
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
            else
            {
                return "Object Not Detected";
            }
        }

        public override void SetConfigString()
        {
            ConfigString = String.Format("Index {0}, between {1} and {2} cm", Index, TopValue.ToString("0.##"), BottomValue.ToString("0.##"));
        }
    }
}
