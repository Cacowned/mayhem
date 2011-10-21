using System.Runtime.Serialization;
using Phidgets.Events;

namespace PhidgetModules
{
    [DataContract]
    public abstract class ValueSensorEventBase : SensorEventBase
    {
        /// <summary>
        /// If this is true, then we want to trigger when
        /// the CurrentValue goes from below the topValue to above the
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

        protected double CurrentValue { get; set; }

        protected double LastValue { get; set; }

        protected override void OnLoadDefaults()
        {
            Increasing = true;
            TopValue = 85;
        }

        protected override void OnAfterLoad()
        {
            CurrentValue = LastValue = TopValue;
        }

        public abstract double Convert(int value);

        protected virtual bool IsValidRange(int value)
        {
            return true;
        }

        protected override void SensorChange(object sender, SensorChangeEventArgs ex)
        {
            // We only care about the index we are watching
            if (ex.Index != Index)
                return;

            if (IsValidRange(ex.Value))
            {
                CurrentValue = Convert(ex.Value);

                if (Increasing && CurrentValue > TopValue && LastValue < TopValue)
                {
                    Trigger();
                }
                else if (!Increasing && CurrentValue < TopValue && LastValue > TopValue)
                {
                    Trigger();
                }

                LastValue = CurrentValue;
            }
        }
    }
}
