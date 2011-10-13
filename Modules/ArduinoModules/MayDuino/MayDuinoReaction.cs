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

        protected override void OnAfterLoad()
        {
            if (manager == null)
                manager = MayDuinoConnectionManager.Instance;
        }

        protected virtual string GetReactionConfigString()
        {
            throw new NotImplementedException();
        }


        public override void Perform()
        {
            throw new NotImplementedException();
        }
    }
}
