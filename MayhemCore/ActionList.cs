using System;
using System.Collections.Generic;

namespace MayhemCore
{
	/// <summary>
	/// Is a list of all the available actions that implement T if they have a configuration window
	/// </summary>
	/// <typeparam name="T">The ModuleType interface that every module must implement</typeparam>
	[Serializable]
	public class ActionList<T> : ModuleList<ActionBase, T>, IEnumerable<ActionBase>
	{
	}
}
