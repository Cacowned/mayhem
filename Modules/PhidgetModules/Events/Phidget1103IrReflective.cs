using System;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemCore.ModuleTypes;
using MayhemWpf.UserControls;
using PhidgetModules.Wpf;

namespace PhidgetModules.Events
{
    [DataContract]
    [MayhemModule("Phidget: Proximity Sensor", "Triggers based on object detection")]
    public class Phidget1103IRReflective : OnOffSensorEventBase, IWpfConfigurable
    {
        protected override void Initialize()
        {
            base.Initialize();

            BottomThreshold = 0;
            TopThreshold = 100;
        }

        public IWpfConfiguration ConfigurationControl
        {
            get { return new Phidget1103IRReflectiveConfig(ifKit, Index, OnTurnOn, ConvertToString); }
        }

        public void OnSaved(IWpfConfiguration configurationControl)
        {
            Index = ((Phidget1103IRReflectiveConfig)configurationControl).Index;
            OnTurnOn = ((Phidget1103IRReflectiveConfig)configurationControl).OnTurnOn;
        }

        public override void SetConfigString()
        {
            string message = "recognizes an object";
            if (!OnTurnOn)
            {
                message = "stops recognizing an object";
            }

            ConfigString = String.Format("Index {0} {1}", Index, message);
        }

        protected string ConvertToString(int value)
        {
            if (value < 100)
                return "Detected";
            else
                return "Not Detected";
        }
    }
}
