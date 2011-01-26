using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using MayhemCore.ModuleTypes;

namespace DefaultModules.Actions
{
    public class Keypress : ActionBase
    {

        public Keypress()
            : base("Key press", "This trigger fires on a predefined key press")
        {
            hasConfig = true;
        }
    }
}
