using System.Runtime.Serialization;
using Phidgets.Events;

namespace PhidgetModules
{
    [DataContract]
    abstract public class RangeSensorEventBase : SensorEventBase
    {
        #region Configuration
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

        #endregion

        protected double value;
        protected double lastValue;

        protected override void Initialize()
        {
            base.Initialize();

            // put it somewhere in the middle
            value = lastValue = TopValue - BottomValue;

            TopValue = 85;
            BottomValue = 20;
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

        protected override void SensorChange(object sender, SensorChangeEventArgs e)
        {
            // We only care about the index we are watching
            if (e.Index != Index)
                return;

            if (IsValidInput(e.Value))
            {
                value = Convert(e.Value);

                if (IsInRange(value) && !IsInRange(lastValue))
                {
                    OnEventActivated();
                }

                lastValue = value;
            }
        }
    }
}
