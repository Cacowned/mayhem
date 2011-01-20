using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MayhemCore
{
    /// <summary>
    /// Main Mayhem class, contains lists of actions, reactions, and runlist
    /// </summary>
    public class Mayhem
    {
        public static ConnectionList ConnectionList
        {
            get;
            private set;
        }
        public static ActionList ActionList
        { 
            get;
            private set;
        }
        public static ReactionList ReactionList
        {
            get;
            private set;
        }

        public Mayhem()
        {
            ConnectionList = new ConnectionList();
            ActionList = new ActionList();
            ReactionList = new ReactionList();
        }
    }
}
