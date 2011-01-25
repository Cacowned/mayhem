using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MayhemCore
{
    /// <summary>
    /// Is a list of all the available actions
    /// </summary>
    /// <typeparam name="T">The ModuleType interface that every module must implement</typeparam>
    public class ActionList<T> : ModuleList<ActionBase, T>, IEnumerable<ActionBase>
    {
    }
}
