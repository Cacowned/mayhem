
using System;
using System.Runtime.Serialization;
namespace MayhemCore
{
    public delegate void EventActivateHandler(object sender, EventArgs e);

    /// <summary>
    /// Base class for all event modules
    /// </summary>
    [DataContract]
    public abstract class EventBase : ModuleBase
    {
        /// <summary>
        /// Event that triggers when the event is activated
        /// </summary>
        public event EventActivateHandler EventActivated;

        public EventBase(string name, string description)
            : base(name, description)
        {
        }

        /// <summary>
        /// Event trigger for when the event is activated. This shouldn't
        /// need to be overridden, just attached to
        /// </summary>
        protected virtual void OnEventActivated()
        {
            EventActivateHandler handler = EventActivated;
            if (handler != null)
            {
                handler(this, null);
            }
        }
    }
}
