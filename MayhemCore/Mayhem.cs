
namespace MayhemCore
{
    /// <summary>
    /// Main Mayhem class, contains lists of actions, reactions, and runlist
    /// </summary>
    public class Mayhem
    {
        public ConnectionList ConnectionList
        {
            get;
            private set;
        }
        public ActionList ActionList
        { 
            get;
            private set;
        }
        public ReactionList ReactionList
        {
            get;
            private set;
        }

        public Mayhem()
        {
            // Set up our three lists
            ConnectionList = new ConnectionList();
            ActionList = new ActionList();
            ReactionList = new ReactionList();
        }
    }
}
