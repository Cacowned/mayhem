using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using MayhemApp.Business_Logic.Zune;

namespace MayhemApp.Business_Logic.Actions.Zune
{
    class LastSongAction : MayhemActionBase, ISerializable
    {
        public LastSongAction(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public LastSongAction(String s) : this() { }

        public LastSongAction()
            : base("Last Song Action",
                     "Goes to the beginning of the song or the last song",
                     "This action tells the Zune Software to go back to the beginning of the song or play the last song.")
        {
        }

        public override void PerformAction(MayhemTriggerBase sender)
        {
            MayhemZune t = MayhemZune.Instance;
            t.SendCommand("^b");
        }
    }
}
