
namespace MayhemCore
{
    /// <summary>
    /// Main Mayhem class, contains lists of actions, reactions, and runlist
    /// </summary>
    /// <typeparam name="T">The interface that modules must implement</typeparam>
    public class Mayhem<T>
    {
        public ConnectionList ConnectionList
        {
            get;
            private set;
        }
        public ActionList<T> ActionList
        { 
            get;
            private set;
        }
        public ReactionList<T> ReactionList
        {
            get;
            private set;
        }

        public Mayhem()
        {
            // Set up our three lists
            ConnectionList = new ConnectionList();
            ActionList = new ActionList<T>();
            ReactionList = new ReactionList<T>();
        }
    }
}
