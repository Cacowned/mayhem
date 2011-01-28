using System;
using System.Runtime.Serialization;

namespace MayhemCore
{
    [Serializable]
    public abstract class ReactionBase: ModuleBase, ISerializable
    {
        public ReactionBase(string name, string description)
            :base(name, description)
        {
        }

        public abstract void Perform();

        #region Serializable
        public ReactionBase(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public new void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);
        }
        #endregion
        
    }
}
