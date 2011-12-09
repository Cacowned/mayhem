using System.Runtime.Serialization;
using Phidgets.Events;

namespace PhidgetModules
{
    [DataContract]
    public abstract class OnOffSensorEventBase : SensorEventBase
    {
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

        private double currentValue;

        private double lastValue;

        protected override void OnLoadDefaults()
        {
            TopThreshold = 900;
            BottomThreshold = 500;
            OnTurnOn = true;
        }

        protected override void SensorChange(object sender, SensorChangeEventArgs ex)
        {
            // We only care about the index we are watching
            if (ex.Index != Index)
                return;

            currentValue = ex.Value;

            if (OnTurnOn && currentValue >= TopThreshold && lastValue < TopThreshold)
            {
                Trigger();
            }
            else if (!OnTurnOn && currentValue <= BottomThreshold && lastValue > BottomThreshold)
            {
                Trigger();
            }

            lastValue = currentValue;
        }
    }
}
