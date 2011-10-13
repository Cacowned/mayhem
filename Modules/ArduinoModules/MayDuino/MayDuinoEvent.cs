using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using System.Runtime.Serialization;
using ArduinoModules.MayDuino;

namespace ArduinoModules.Events
{
    [DataContract]
    public class MayduinoEventBase : EventBase
    {
        protected MayDuinoConnectionManager manager = MayDuinoConnectionManager.Instance;

        protected   Event_t EventType
        {
            get
            {
                return Event_t.EVENT_DISABLED;
            }
        }


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
