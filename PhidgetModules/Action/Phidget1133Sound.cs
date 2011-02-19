using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore.ModuleTypes;
using Phidgets.Events;
using System.Runtime.Serialization;
using System.Windows;

namespace PhidgetModules.Action
{
    [Serializable]
    public class Phidget1133Sound: SensorActionBase, IWpf, ISerializable
    {
        


        /// <summary>
        /// If this is true, then we want to trigger when
        /// the value goes from below the topValue to above the
        /// topValue. If it is false, then we trigger when we go
        /// from above to below
        /// </summary>
        bool increasing = true;

        double topValue = 85;

        double value;

        double lastValue;

        public Phidget1133Sound()
            : base("Phidget-1133: Sound Sensor", "Triggers at a certain decibel level")
        {
            hasConfig = true;
            increasing = true;
            topValue = 85;

            Setup();
        }

        protected override void Setup()
        {
            base.Setup();
            value = lastValue = topValue;

            SetConfigString();
        }

        public void WpfConfig()
        {
            var window = new Phidget1133SoundConfig(ifKit, index, topValue, increasing);
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            if (window.ShowDialog() == true)
            {
                index = window.index;
                topValue = window.topValue;
                increasing = window.increasing;

                SetConfigString();
            }
        }

        protected void SetConfigString()
        {
            string overUnder = "over";
            if (!increasing)
            {
                overUnder = "under";
            }

            ConfigString = String.Format("Index {0} goes {1} {2} dB", index, overUnder, topValue.ToString("0.###"));
        }

        protected override void SensorChange(object sender, SensorChangeEventArgs e)
        {
            // We only care about the index we are watching
            if (e.Index != index)
                return;

            value = Convert(e.Value);

            if (increasing && value > topValue && lastValue < topValue)
            {
                OnActionActivated();
            }
            else if (!increasing && value < topValue && lastValue > topValue)
            {
                OnActionActivated();
            }

            lastValue = value;
        }

        public static double Convert(int value)
        {
            return 16.801 * Math.Log(value) + 9.872;
        }

        #region Serialization

        public Phidget1133Sound(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Setup();

        }

        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);


        }
        #endregion
    }
}
