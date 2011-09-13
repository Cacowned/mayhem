using System;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemCore.ModuleTypes;
using MayhemDefaultStyles.UserControls;
using PhidgetModules.Wpf;

namespace PhidgetModules.Events
{
    [DataContract]
    [MayhemModule("Phidget: Sound Sensor", "Triggers at a certain decibel level")]
    public class Phidget1133Sound : ValueSensorEventBase, IWpfConfigurable
    {
        public IWpfConfiguration ConfigurationControl
        {
            get { return new Phidget1133SoundConfig(ifKit, Index, TopValue, Increasing, ConvertToString); }
        }

        public void OnSaved(IWpfConfiguration configurationControl)
        {
            Index = ((Phidget1133SoundConfig)configurationControl).Index;
            TopValue = ((Phidget1133SoundConfig)configurationControl).TopValue;
            Increasing = ((Phidget1133SoundConfig)configurationControl).Increasing;
            SetConfigString();
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
