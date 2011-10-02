using System.Runtime.Serialization;

namespace MayhemCore
{
    [DataContract]
    public abstract class ReactionBase : ModuleBase
    {
        public abstract void Perform();
    }
}
