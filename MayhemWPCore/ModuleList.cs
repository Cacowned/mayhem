using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MayhemCore
{
    /// <summary>
    /// Helper methods for Action and Reaction lists
    /// </summary>
    /// <typeparam name="T">ActionBase or ReactionBase</typeparam>
    /// <typeparam name="V">The interface type that modules must implement</typeparam>
    public abstract class ModuleList<T, V> : List<T> where T : ModuleBase
    {
        public ModuleList()
        {
            
        }
    }
}
