using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MayhemCore
{
    /// <summary>
    /// Base class for all action modules
    /// </summary>
    public abstract class ActionBase: ModuleBase
    {
        public ActionBase(string name, string description)
            :base(name, description)
        {

        }
    }
}
