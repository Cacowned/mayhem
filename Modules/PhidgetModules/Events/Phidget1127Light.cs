using System;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemCore.ModuleTypes;
using MayhemWpf.UserControls;
using PhidgetModules.Wpf;

namespace PhidgetModules.Events
{
    [DataContract]
    [MayhemModule("Phidget: Light Sensor", "Triggers at a certain light level")]
    public class Phidget1127Light : ValueSensorEventBase, IWpfConfigurable
    {
        public IWpfConfiguration ConfigurationControl
        {
            get { return new Phidget1127LightConfig(ifKit, Index, TopValue, Increasing, ConvertToString); }
        }

        public void OnSaved(IWpfConfiguration configurationControl)
        {
            Index = ((Phidget1127LightConfig)configurationControl).Index;
            TopValue = ((Phidget1127LightConfig)configurationControl).TopValue;
            Increasing = ((Phidget1127LightConfig)configurationControl).Increasing;
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
