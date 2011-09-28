using System.Runtime.Serialization;
using Phidgets.Events;

namespace PhidgetModules
{
    [DataContract]
    abstract public class OnOffSensorEventBase : SensorEventBase
    {
        #region Configuration
        [DataMember]
        protected int TopThreshold;

        [DataMember]
        protected int BottomThreshold;

        // If this is true, then we want to trigger
        // when this sensor turns "on" otherwise
        // trigger when this sensor turns "off"
        [DataMember]
        protected bool OnTurnOn;

        #endregion

        protected double CurrentValue { get; set; }
        protected double LastValue { get; set; }

        public OnOffSensorEventBase()
        {
            TopThreshold = 900;
            BottomThreshold = 500;
            OnTurnOn = true;
        }

        protected override void Initialize()
        {
            base.Initialize();
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
