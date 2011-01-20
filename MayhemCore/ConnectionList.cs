using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace MayhemCore
{
    /// <summary>
    /// Maintains a set of all the current
    /// connections
    /// </summary>
    public class ConnectionList: IEnumerable<Connection>
    {
        List<Connection> connectionList;

        public ConnectionList()
        {
            connectionList = new List<Connection>();
        }

        public IEnumerator GetEnumerator()
        {
            return connectionList.GetEnumerator();
        }

        IEnumerator<Connection> IEnumerable<Connection>.GetEnumerator()
        {
            return connectionList.GetEnumerator();
        }
    }
}
