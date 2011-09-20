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
    [MayhemModule("Phidget: Touch Sensor", "Triggers based on touching the sensor")]
    public class Phidget1129Touch : OnOffSensorEventBase, IWpfConfigurable
    {
        public IWpfConfiguration ConfigurationControl
        {
            get { return new SensorConfig(IfKit, Index, ConvertToString, new Config1129Touch(OnTurnOn)); }
        }

        public void OnSaved(IWpfConfiguration configurationControl)
        {
            SensorConfig sensor = configurationControl as SensorConfig;
            Config1129Touch config = sensor.Sensor as Config1129Touch;

            Index = sensor.Index;
            OnTurnOn = config.OnTurnOn;
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
