using System.Runtime.Serialization;
using Phidgets.Events;

namespace PhidgetModules
{
    [DataContract]
    abstract public class RangeSensorEventBase : SensorEventBase
    {
        // Define the range we care about
        [DataMember]
        protected double TopValue;

        [DataMember]
        protected double BottomValue;

        protected double CurrentValue { get; set; }
        protected double LastValue { get; set; }

        protected override void OnLoadDefaults()
        {
            TopValue = 85;
            BottomValue = 20;
        }

        protected override void OnAfterLoad()
        {
            // put it somewhere in the middle
            CurrentValue = LastValue = TopValue - BottomValue;
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
                CurrentValue = Convert(ex.Value);

                if (IsInRange(CurrentValue) && !IsInRange(LastValue))
                {
                    Trigger();
                }

                LastValue = CurrentValue;
            }
        }
    }
}
