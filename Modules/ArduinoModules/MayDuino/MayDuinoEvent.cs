using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using System.Runtime.Serialization;

namespace ArduinoModules.Events
{
    [DataContract]
    public class MayduinoEventBase : EventBase
    {
        protected MayDuinoConnectionManager manager = MayDuinoConnectionManager.Instance;


        protected override void OnAfterLoad()
        {
            if (manager == null)
                manager = MayDuinoConnectionManager.Instance;
        }

   

        protected virtual string GetEventConfigString()
        {
            throw new NotImplementedException();
        }
    }
}
