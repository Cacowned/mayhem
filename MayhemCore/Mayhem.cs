namespace MayhemCore
{
	/// <summary>
	/// Main Mayhem class, contains lists of actions, reactions, and run list
	/// </summary>
	/// <typeparam name="T">The interface that modules must implement</typeparam>
	public class Mayhem<T>
	{
		public ConnectionList ConnectionList {
			get;
			set;
		}
		public ActionList<T> ActionList {
			get;
			set;
		}
		public ReactionList<T> ReactionList {
			get;
			set;
		}

		public Mayhem() {
			// Set up our three lists
			ActionList = new ActionList<T>();
			ReactionList = new ReactionList<T>();
			ConnectionList = new ConnectionList();
		}
	}
}
