using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;

namespace MayhemCore
{
    /// <summary>
    /// A list of all the avaliable reactions
    /// </summary>
    public class ReactionList<T> : ModuleList<ReactionBase, T>, IEnumerable<ReactionBase>
    {
    }
}
