using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using System.Diagnostics;

namespace DefaultModules.Reactions.Debugging
{
    public class Message: ReactionBase
    {
        public Message()
            :base("Debug Message", "Generates debug output when triggered")
        {

        }
        public override void Perform()
        {
            Debug.WriteLine("Debug Reaction got triggered");
        }
    }
}
