using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.IO;
using System.Collections.Generic;
using System.Xml;

namespace MayhemCore
{
    /// <summary>
    /// Maintains a set of all the current connections
    /// </summary>
    public class ConnectionList : ObservableCollection<Connection>
    {
        public static ConnectionList Deserialize(Stream stream, List<Type> types)
        {
            ConnectionList obj = null;
            /* 
             * Deserialize from the stream
             */
            return obj;
        }

        public void Serialize(Stream stream)
        {
            /*
             * Serialize our object into the stream
             */
        }
    }
}
