using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace MayhemCore
{
	/// <summary>
	/// Maintains a set of all the current connections
	/// </summary>
	[Serializable]
	public class ConnectionList : ObservableCollection<Connection>
	{
	}
}
