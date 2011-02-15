using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MayhemCore
{
    /// <summary>
    /// Maintains a set of all the current connections
    /// </summary>
    [Serializable]
    public class ConnectionList : ObservableCollection<Connection>
    {
    }
}
