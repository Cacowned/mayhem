using System;
using System.Runtime.Serialization;

namespace MayhemCore
{
    /// <summary>
    /// Base class for all event modules
    /// </summary>
    [DataContract]
    public abstract class EventBase : ModuleBase
    {
        /// <summary>
        /// Event trigger for when the event is activated. This shouldn't
        /// need to be overridden, just attached to
        /// </summary>
        protected virtual void Trigger()
        {
            Connection.Trigger();
        }
    }
}
