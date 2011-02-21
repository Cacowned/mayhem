using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore.ModuleTypes;
using Phidgets.Events;
using System.Runtime.Serialization;
using System.Windows;
using PhidgetModules.Wpf;

namespace PhidgetModules.Action
{
    [Serializable]
    public class Phidget1133Sound: ValueSensorActionBase, IWpf, ISerializable
    {
        
        public Phidget1133Sound()
            : base("Phidget-1133: Sound Sensor", "Triggers at a certain decibel level")
        {
            Setup();
        }

        public void WpfConfig()
        {
            var window = new Phidget1133SoundConfig(ifKit, index, topValue, increasing, Convert);
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            if (window.ShowDialog() == true)
            {
                index = window.index;
                topValue = window.topValue;
                increasing = window.increasing;

                SetConfigString();
            }
        }

        public override double Convert(int value) {
            return 16.801 * Math.Log(value) + 9.872;
        }

        protected override void SetConfigString()
        {
            string overUnder = "above";
            if (!increasing)
            {
                overUnder = "below";
            }

            ConfigString = String.Format("Index {0} goes {1} {2} dB", index, overUnder, topValue.ToString("0.###"));
        }


        #region Serialization

        public Phidget1133Sound(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            
        }

        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

        }
        #endregion
    }
}
