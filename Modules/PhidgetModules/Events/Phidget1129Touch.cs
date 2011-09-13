using System;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemCore.ModuleTypes;
using MayhemDefaultStyles.UserControls;
using PhidgetModules.Wpf;

namespace PhidgetModules.Events
{
    [DataContract]
    [MayhemModule("Phidget: Touch Sensor", "Triggers based on touching the sensor")]
    public class Phidget1129Touch : OnOffSensorEventBase, IWpfConfigurable
    {
        public IWpfConfiguration ConfigurationControl
        {
            get { return new Phidget1129TouchConfig(ifKit, Index, OnTurnOn, ConvertToString); }
        }

        public void OnSaved(IWpfConfiguration configurationControl)
        {
            Index = ((Phidget1129TouchConfig)configurationControl).Index;
            OnTurnOn = ((Phidget1129TouchConfig)configurationControl).OnTurnOn;
            SetConfigString();
        }

        public string ConvertToString(int value)
        {
            if (value > 500)
                return "Touch Detected";
            else
                return "No Touch Detected";
        }

        public override void SetConfigString()
        {
            string message = "turns on";
            if (!OnTurnOn)
            {
                message = "turns off";
            }

            ConfigString = String.Format("Index {0} {1}", Index, message);
        }
    }
}
