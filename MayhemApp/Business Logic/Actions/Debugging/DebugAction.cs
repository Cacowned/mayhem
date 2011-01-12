using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace MayhemApp.Business_Logic.Actions.Debugging
{
    [Serializable]
    class DebugAction : MayhemActionBase, ISerializable
    {


        public DebugAction(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public DebugAction(String s) : this() { }

        public DebugAction()
            : base("Debug Action",
                     "Generates debug output when triggered",
                     "This action generates debug output when triggered. Mainly for devloper use.")
        {


        }

        public override void PerformAction(MayhemTriggerBase sender)
        {
            Debug.WriteLine("DebugAction " + ID + " got triggered!");
        }


    }
}
