using System.Runtime.Serialization;
using MayhemCore;
using Phidgets;
using Phidgets.Events;

namespace PhidgetModules
{
    [DataContract]
    public abstract class SensorEventBase : EventBase
    {
        // The interface kit we are using for the sensors
        protected static InterfaceKit IfKit
        {
            get;
            private set;
        }

        // Which index do we want to be looking at?
        [DataMember]
        protected int Index
        {
            get;
            set;
        }

        private SensorChangeEventHandler handler;

        protected override void OnBeforeLoad()
        {
            IfKit = InterfaceFactory.Interface;
        }

        protected override void OnLoadDefaults()
        {
            // Default to first index
            Index = 0;
        }

        protected override void OnAfterLoad()
        {
            handler = SensorChange;
        }

        protected abstract void SensorChange(object sender, SensorChangeEventArgs ex);

        protected override void OnEnabling(EnablingEventArgs e)
        {
            IfKit.SensorChange += handler;
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            if (IfKit != null)
            {
                IfKit.SensorChange -= handler;
            }
        }
    }
}
