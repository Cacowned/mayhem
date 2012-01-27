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

        // This is necessary to invert the effect of the configuration.
        // For example, the touch sensor is "detecting" when the value is
        // over 800 and below that is not detected.
        // However, for the ir reflective, it is "detected" when you are close (lower value)
        // thus we must invert the way we trigger in that case.
        protected bool invert;

        private double currentValue;

        private double lastValue;

        protected override void OnBeforeLoad()
        {
            base.OnBeforeLoad();

            invert = false;
        }

        protected override void OnLoadDefaults()
        {
			base.OnLoadDefaults();

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

            bool shouldTrigger = false;

            // Take the xor of whether we should trigger and whether to invert the result
            if (OnTurnOn ^ invert && currentValue >= TopThreshold && lastValue < TopThreshold)
            {
                shouldTrigger = true;
            }
            else if (!OnTurnOn ^ invert && currentValue <= BottomThreshold && lastValue > BottomThreshold)
            {
                shouldTrigger = true;
            }
            
            if (shouldTrigger)
            {
                Trigger();
            }

            lastValue = currentValue;
        }
    }
}
