using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using System.Diagnostics;

namespace DefaultModules.Reactions.Debugging
{
    public class Popup : ReactionBase
    {
        public Popup()
            : base("Popup Window", "Generates a small popup window when triggered")
        {

        }
        public override void Perform()
        {
            Debug.WriteLine("Debug Reaction got triggered");
        }
    }
}
