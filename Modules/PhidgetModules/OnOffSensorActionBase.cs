using System.Runtime.Serialization;
using Phidgets.Events;

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

        protected double value;
        protected double lastValue;

        protected override void Initialize()
        {
            base.Initialize();

            TopThreshold = 900;
            BottomThreshold = 500;
            OnTurnOn = true;
        }

        protected override void SensorChange(object sender, SensorChangeEventArgs e)
        {
            // We only care about the index we are watching
            if (e.Index != Index)
                return;

            value = e.Value;

            if (OnTurnOn && value >= TopThreshold && lastValue < TopThreshold)
            {
                OnEventActivated();
            }
            else if (!OnTurnOn && value <= BottomThreshold && lastValue > BottomThreshold)
            {
                OnEventActivated();
            }

            lastValue = value;
        }
    }
}
