using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using Phidgets.Events;

namespace PhidgetModules
{
    [Serializable]
    abstract public class RangeSensorActionBase : SensorActionBase, ISerializable
    {
        // Define the range we care about
        protected double topValue = 85;
        protected double bottomValue = 20;

        protected double value;
        protected double lastValue;

        public RangeSensorActionBase(string name, string description)
            : base(name, description)
        {

            topValue = 85;
            bottomValue = 20;

            Setup();
        }

        protected override void Setup()
        {
            base.Setup();

            hasConfig = true;

            // put it somewhere in the middle
            value = lastValue = topValue-bottomValue;

            SetConfigString();
        }

        protected abstract void SetConfigString();

        public abstract double Convert(int value);

        protected virtual bool IsValidInput(int value)
        {
            return true;
        }

        protected bool IsInRange(double value)
        {
            return value > bottomValue && value < topValue;
        }

        protected override void SensorChange(object sender, SensorChangeEventArgs e)
        {
            // We only care about the index we are watching
            if (e.Index != index)
                return;

            if (IsValidInput(e.Value))
            {
                value = Convert(e.Value);

                if (IsInRange(value) && !IsInRange(lastValue))
                {
                    OnActionActivated();
                }
                
                lastValue = value;
            }
        }

        #region Serialization

        public RangeSensorActionBase(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            bottomValue = info.GetDouble("BottomValue");
            topValue = info.GetDouble("TopValue");

            Setup();
        }

        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("BottomValue", bottomValue);
            info.AddValue("TopValue", topValue);
        }
        #endregion
    }
}
