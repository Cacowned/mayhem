using System;
using System.Runtime.Serialization;

namespace MayhemCore
{
    [DataContract]
	public abstract class ReactionBase : ModuleBase
	{
		public ReactionBase(string name, string description)
			: base(name, description) {
		}

		public abstract void Perform();
	}
}
