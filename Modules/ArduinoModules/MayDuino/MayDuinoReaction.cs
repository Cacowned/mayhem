using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;

namespace ArduinoModules.Events
{
    public abstract class MayduinoReactionBase: ReactionBase
    {
        private MayDuinoConnectionManager manager = MayDuinoConnectionManager.Instance;

        protected abstract string GetReactionConfigString();
        
    }
}
