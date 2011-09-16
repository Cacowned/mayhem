
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
        /// Event that triggers when the event is activated
        /// </summary>
        public event EventHandler EventActivated;

        /// <summary>
        /// Event trigger for when the event is activated. This shouldn't
        /// need to be overridden, just attached to
        /// </summary>
        protected virtual void OnEventActivated()
        {
            EventActivated(this, null);
        }
    }
}
