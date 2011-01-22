using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MayhemCore
{
    /// <summary>
    /// Is a list of all the avaliable actions
    /// </summary>
    public class ActionList<T> : ModuleList<ActionBase, T>, IEnumerable<ActionBase>
    {

    }
}
