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

        protected virtual  Event_t EventType
        {
            get
            {
                return Event_t.EVENT_DISABLED;
            }
        }

        protected override void OnEnabling(EnablingEventArgs e)
        { 
            if (!e.WasConfiguring)
                manager.EventEnabled(this);
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            if (!e.IsConfiguring)
                manager.EventDisabled(this);
        }

        protected override void OnAfterLoad()
        {
            if (manager == null)
                manager = MayDuinoConnectionManager.Instance;
        }
  
        public virtual string EventConfigString
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
