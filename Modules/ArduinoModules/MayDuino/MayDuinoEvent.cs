using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;

namespace ArduinoModules.Events
{
    public class MayduinoEventBase : EventBase
    {
        private MayDuinoConnectionManager manager = MayDuinoConnectionManager.Instance;

        protected virtual string GetEventConfigString()
        {
            throw new NotImplementedException();
        }
    }
}
