using System;
using System.Runtime.Serialization;
namespace MayhemCore
{
    /// <summary>
    /// Main Mayhem class, contains lists of actions, reactions, and run list
    /// </summary>
    /// <typeparam name="T">The interface that modules must implement</typeparam>
    [Serializable]
    public class Mayhem<T>: ISerializable
    {
        public ConnectionList ConnectionList
        {
            get;
            set;
        }
        public ActionList<T> ActionList
        { 
            get;
            set;
        }
        public ReactionList<T> ReactionList
        {
            get;
            set;
        }

        public Mayhem()
        {
            // Set up our three lists
            ActionList = new ActionList<T>();
            ReactionList = new ReactionList<T>();
            ConnectionList = new ConnectionList();
        }

        #region Serialization
        public Mayhem(SerializationInfo info, StreamingContext context)
        {
            ActionList = new ActionList<T>();
            ReactionList = new ReactionList<T>();
            ConnectionList = (ConnectionList)info.GetValue("ConnectionList", typeof(ConnectionList));
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context) {
            info.AddValue("ConnectionList", ConnectionList);
        }
        #endregion
    }
}
