using System.Runtime.Serialization;
using Phidgets.Events;
using System;

namespace PhidgetModules
{
    [DataContract]
    abstract public class OnOffSensorEventBase : SensorEventBase
    {
        #region Configuration
        [DataMember]
        protected int TopThreshold
        {
            get;
            set;
        }

        [DataMember]
        protected int BottomThreshold
        {
            get;
            set;
        }

        // If this is true, then we want to trigger
        // when this sensor turns "on" otherwise
        // trigger when this sensor turns "off"
        [DataMember]
        protected bool OnTurnOn
        {
            get;
            set;
        }

        #endregion

        protected double CurrentValue { get; set; }
        protected double LastValue { get; set; }

        protected override void Initialize()
        {
            base.Initialize();

            TopThreshold = 900;
            BottomThreshold = 500;
            OnTurnOn = true;
        }

        protected override void SensorChange(object sender, SensorChangeEventArgs ex)
        {
            // We only care about the index we are watching
            if (ex.Index != Index)
                return;

            CurrentValue = ex.Value;

            if (OnTurnOn && CurrentValue >= TopThreshold && LastValue < TopThreshold)
            {
                Trigger();
            }
            else if (!OnTurnOn && CurrentValue <= BottomThreshold && LastValue > BottomThreshold)
            {
                Trigger();
            }

            LastValue = CurrentValue;
        }
    }
}
