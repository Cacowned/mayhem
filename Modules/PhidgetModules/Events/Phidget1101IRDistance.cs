using System;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemCore.ModuleTypes;
using MayhemDefaultStyles.UserControls;
using PhidgetModules.Wpf;

namespace PhidgetModules.Events
{
    [DataContract]
    [MayhemModule("Phidget: IR Distance", "Triggers at a certain distance")]
    public class Phidget1101IRDistance : RangeSensorEventBase, IWpfConfigurable
    {
        public IWpfConfiguration ConfigurationControl
        {
            get { return new Phidget1101IRDistanceConfig(ifKit, Index, TopValue, BottomValue, ConvertToString); }
        }

        public void OnSaved(IWpfConfiguration configurationControl)
        {
            Index = ((Phidget1101IRDistanceConfig)configurationControl).Index;
            TopValue = ((Phidget1101IRDistanceConfig)configurationControl).TopValue;
            BottomValue = ((Phidget1101IRDistanceConfig)configurationControl).BottomValue;
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
