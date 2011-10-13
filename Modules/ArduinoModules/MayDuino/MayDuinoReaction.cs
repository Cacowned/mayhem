using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using System.Runtime.Serialization;

namespace ArduinoModules.Events
{
    [DataContract]
    public class MayduinoReactionBase: ReactionBase
    {
        protected MayDuinoConnectionManager manager = MayDuinoConnectionManager.Instance;

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
