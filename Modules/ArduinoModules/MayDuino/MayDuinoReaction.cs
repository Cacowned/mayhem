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
    public class MayduinoReactionBase: ReactionBase
    {
        protected MayDuinoConnectionManager manager = MayDuinoConnectionManager.Instance;

        protected virtual Reaction_t ReactionType
        {
            get
            {
                return Reaction_t.REACTION_DISABLED;
            }
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
            if (!e.WasConfiguring)
                manager.ReactionEnabled(this);
        }

              
        protected override void OnAfterLoad()
        {
            if (manager == null)
                manager = MayDuinoConnectionManager.Instance;
        }

        public virtual string ReactionConfigString
        {
            get
            {
                throw new NotImplementedException();
            }
        }


        public override void Perform()
        {
            throw new NotImplementedException();
        }
    }
}
