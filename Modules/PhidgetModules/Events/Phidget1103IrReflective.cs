﻿using System;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemCore.ModuleTypes;
using MayhemWpf.UserControls;
using PhidgetModules.Wpf;
using PhidgetModules.Wpf.UserControls;

namespace PhidgetModules.Events
{
    [DataContract]
    [MayhemModule("Phidget: Proximity Sensor", "Triggers based on object detection")]
    public class Phidget1103IRReflective : OnOffSensorEventBase, IWpfConfigurable
    {
        protected override void Initialize()
        {
            BottomThreshold = 0;
            TopThreshold = 100;
        }

        public WpfConfiguration ConfigurationControl
        {
            get { return new SensorConfig(IfKit, Index, ConvertToString, new Config1103IRReflective(OnTurnOn)); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            SensorConfig sensor = configurationControl as SensorConfig;
            Config1103IRReflective config = sensor.Sensor as Config1103IRReflective;

            Index = sensor.Index;
            OnTurnOn = config.OnTurnOn;
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
