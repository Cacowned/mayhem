using System;
using System.Collections.Generic;

namespace MayhemCore
{
	/// <summary>
	/// A list of all the available reactions
	/// </summary>
	[Serializable]
	public class ReactionList<T> : ModuleList<ReactionBase, T>, IEnumerable<ReactionBase>
	{
	}
}
