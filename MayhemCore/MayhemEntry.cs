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
    public class MayhemEntry
    {
        public ConnectionList ConnectionList
        {
            get;
            private set;
        }
        public EventList EventList
        {
            get;
            private set;
        }
        public ReactionList ReactionList
        {
            get;
            private set;
        }

        public Type ConfigurableType
        {
            get;
            private set;
        }

        private static MayhemEntry _instance;

        public static MayhemEntry Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new MayhemEntry();
                }

                return _instance;
            }
        }

        public event EventHandler ShuttingDown;

        private MayhemEntry()
        {
            // Set up our three lists
            EventList = new EventList();
            ReactionList = new ReactionList();
            ConnectionList = new ConnectionList();
        }

        /// <summary>
        /// Sets the type that modules must implement if they are configurable to be added to the
        /// module lists
        /// </summary>
        /// <param name="configType"></param>
        public void SetConfigurationType(Type configType)
        {
            if (this.ConfigurableType != null) {
                throw new InvalidOperationException("Configuration type has already been set");
            }
            else {
                this.ConfigurableType = configType;
            }
        }

        /// <summary>
        /// Loads the passed ConnectionList by adding each element
        /// into this instance's connectionList
        /// </summary>
        /// <param name="connections"></param>
        public void LoadConnections(ConnectionList connections)
        {
            if (connections == null)
            {
                throw new ArgumentNullException("connections");
            }

            foreach (Connection c in connections)
            {
                ConnectionList.Add(c);
            }
        }

        public void Shutdown()
        {
            if (ShuttingDown != null)
            {
                ShuttingDown(this, null);
            }
        }
    }
}
