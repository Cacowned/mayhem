using System.Runtime.Serialization;
using Phidgets.Events;

namespace PhidgetModules
{
    [DataContract]
    abstract public class ValueSensorEventBase : SensorEventBase
    {
        #region Configuration
        /// <summary>
        /// If this is true, then we want to trigger when
        /// the value goes from below the topValue to above the
        /// topValue. If it is false, then we trigger when we go
        /// from above to below
        /// </summary>
        [DataMember]
        protected bool Increasing
        {
            get;
            set;
        }

        [DataMember]
        protected double TopValue
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

            Increasing = true;
            TopValue = 85;

            value = lastValue = TopValue;
        }

        public abstract double Convert(int value);

        protected virtual bool IsValidRange(int value)
        {
            return true;
        }

        protected override void SensorChange(object sender, SensorChangeEventArgs e)
        {
            // We only care about the index we are watching
            if (e.Index != Index)
                return;

            if (IsValidRange(e.Value))
            {
                value = Convert(e.Value);

                if (Increasing && value > TopValue && lastValue < TopValue)
                {
                    Trigger();
                }
                else if (!Increasing && value < TopValue && lastValue > TopValue)
                {
                    Trigger();
                }

                lastValue = value;
            }
        }
    }
}
