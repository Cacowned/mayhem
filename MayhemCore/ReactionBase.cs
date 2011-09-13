using System;
using System.Runtime.Serialization;

namespace MayhemCore
{
    [DataContract]
    public abstract class ReactionBase : ModuleBase
	{
        public ReactionBase()
        {
        }

		public abstract void Perform();
	}
}
