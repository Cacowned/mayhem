using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MayhemCore
{
    public abstract class ReactionBase: ModuleBase
    {
        public ReactionBase(string name, string description)
            :base(name, description)
        {
        }

        public abstract void Perform();
    }
}
