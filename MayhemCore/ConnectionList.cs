using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace MayhemCore
{
	/// <summary>
	/// Maintains a set of all the current connections
	/// </summary>
	[DataContract]
	public class ConnectionList : ObservableCollection<Connection>
	{
	}
}
