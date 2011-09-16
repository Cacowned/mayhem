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
        protected int Index
        {
            get;
            set;
        }

        #endregion

        // The interface kit we are using for the sensors
        protected static InterfaceKit ifKit;

        protected SensorChangeEventHandler handler;

        protected override void Initialize()
        {
            base.Initialize();

            // If we don't have an ifKit yet, create one
            if (ifKit == null)
            {
                ifKit = InterfaceFactory.GetInterface();
            }

            handler = new SensorChangeEventHandler(SensorChange);

            // Default to first index
            Index = 0;
        }

        protected abstract void SensorChange(object sender, SensorChangeEventArgs e);

        public override void Enable()
        {
            base.Enable();
            ifKit.SensorChange += handler;
        }

        public override void Disable()
        {
            base.Disable();

            if (ifKit != null)
            {
                ifKit.SensorChange -= handler;
            }

        }
    }
}
