using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using System.Diagnostics;
using MayhemCore.ModuleTypes;
using System.Runtime.Serialization;

namespace DefaultModules.Reactions.Debugging
{
    [Serializable]
    public class Message: ReactionBase
    {
        public Message()
            :base("Debug: Message", "Generates debug output when triggered")
        {
        }
        public override void Perform()
        {
            Debug.WriteLine("{0}: Debug Reaction got triggered", DateTime.Now.ToLongTimeString());
        }
    }
}
