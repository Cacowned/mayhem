using System.Runtime.Serialization;

namespace MayhemCore
{
    /// <summary>
    /// The base class for all Mayhem reactions
    /// </summary>
    [DataContract]
    public abstract class ReactionBase : ModuleBase
    {
        /// <summary>
        /// This method is called when the connection is triggered. The reaction
        /// should take action here.
        /// </summary>
        public abstract void Perform();
    }
}
