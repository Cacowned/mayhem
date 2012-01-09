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
        /// Event trigger for when the event is activated.
        /// </summary>
        protected void Trigger()
        {
            Connection.Trigger();
        }
    }
}
