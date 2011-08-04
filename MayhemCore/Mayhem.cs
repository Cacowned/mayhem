using System;
namespace MayhemCore
{
	/// <summary>
	/// Main Mayhem class, contains lists of events, reactions, and list of connections
	/// 
	/// When making a new app that uses Mayhem, you should create an instance of this class
	/// </summary>
	/// <typeparam name="T">
	/// The interface that modules must implement if they have
	/// a configuration window. If they don't implement a configuration
	/// window, they don't have to implement this class
	/// </typeparam>
	public class Mayhem<T>
	{
		public ConnectionList ConnectionList {
			get;
			private set;
		}
		public EventList<T> EventList {
			get;
			private set;
		}
		public ReactionList<T> ReactionList {
			get;
			private set;
		}

		public Mayhem() {
			// Set up our three lists
			EventList = new EventList<T>();
			ReactionList = new ReactionList<T>();
			ConnectionList = new ConnectionList();
		}

		/// <summary>
		/// Loads the passed ConnectionList by adding each element
		/// into this instance's connectionList
		/// </summary>
		/// <param name="connections"></param>
		public void LoadConnections(ConnectionList connections) {
			if (connections == null) {
				throw new ArgumentNullException("connections");
			}

			foreach (Connection c in connections) {
				ConnectionList.Add(c);
			}
		}
	}
}
