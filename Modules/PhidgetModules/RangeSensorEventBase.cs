using System.Runtime.Serialization;
using Phidgets.Events;

namespace PhidgetModules
{
    [DataContract]
    public abstract class RangeSensorEventBase : SensorEventBase
    {
        // Define the range we care about
        [DataMember]
        protected double TopValue
        {
            get;
            set;
        }

        [DataMember]
        protected double BottomValue
        {
            get;
            set;
        }

        private double currentValue;

        private double lastValue;

        protected override void OnLoadDefaults()
        {
            TopValue = 85;
            BottomValue = 20;
        }

        protected override void OnAfterLoad()
        {
            // put it somewhere in the middle
            currentValue = lastValue = TopValue - BottomValue;
        }

        public abstract double Convert(int value);

        protected virtual bool IsValidInput(int value)
        {
            return true;
        }

        protected bool IsInRange(double value)
        {
            return value > BottomValue && value < TopValue;
        }

        protected override void SensorChange(object sender, SensorChangeEventArgs ex)
        {
            // We only care about the index we are watching
            if (ex.Index != Index)
                return;

            if (IsValidInput(ex.Value))
            {
                currentValue = Convert(ex.Value);

                if (IsInRange(currentValue) && !IsInRange(lastValue))
                {
                    Trigger();
                }

                lastValue = currentValue;
            }
        }
    }
}
