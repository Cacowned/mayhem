using System;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemCore.ModuleTypes;
using MayhemWpf.UserControls;
using PhidgetModules.Wpf;

namespace PhidgetModules.Events
{
    [DataContract]
    [MayhemModule("Phidget: Force Sensor", "Triggers at a certain force level")]
    public class Phidget1106Force : ValueSensorEventBase, IWpfConfigurable
    {
        public IWpfConfiguration ConfigurationControl
        {
            get { return new Phidget1106ForceConfig(IfKit, Index, TopValue, Increasing, ConvertToString); }
        }

        public void OnSaved(IWpfConfiguration configurationControl)
        {
            Index = ((Phidget1106ForceConfig)configurationControl).Index;
            TopValue = ((Phidget1106ForceConfig)configurationControl).TopValue;
            Increasing = ((Phidget1106ForceConfig)configurationControl).Increasing;
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
