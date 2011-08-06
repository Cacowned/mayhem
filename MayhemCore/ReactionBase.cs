using System;
using System.Runtime.Serialization;

namespace MayhemCore
{
	public abstract class ReactionBase : ModuleBase
	{
        public ReactionBase()
        {
        }

		public abstract void Perform();
	}
}
