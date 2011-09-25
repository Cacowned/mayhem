using System.Runtime.Serialization;
using MayhemCore;
using Phidgets;
using Phidgets.Events;

namespace PhidgetModules
{
    [DataContract]
    abstract public class SensorEventBase : EventBase
    {
        #region Configuration
        // Which index do we want to be looking at?
        [DataMember]
        protected int Index;

        #endregion

        // The interface kit we are using for the sensors
        protected static InterfaceKit IfKit { get; private set; }

        private SensorChangeEventHandler handler;

        protected override void Initialize()
        {
            // If we don't have an IfKit yet, create one
            if (IfKit == null)
            {
                IfKit = InterfaceFactory.Interface;
            }

            handler = new SensorChangeEventHandler(SensorChange);

            // Default to first index
            Index = 0;
        }

        protected abstract void SensorChange(object sender, SensorChangeEventArgs ex);

        public override bool Enable()
        {
            IfKit.SensorChange += handler;

            return true;
        }

        public override void Disable()
        {
            if (IfKit != null)
            {
                IfKit.SensorChange -= handler;
            }

        }
    }
}
