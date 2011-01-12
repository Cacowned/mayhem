using System;
using System.Runtime.Serialization;
using MayhemApp.Business_Logic.Zune;

namespace MayhemApp.Business_Logic.Actions.Zune
{
    class PlayPauseAction : MayhemActionBase, ISerializable
    {
        public PlayPauseAction(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public PlayPauseAction(String s) : this() { }

        public PlayPauseAction()
            : base("Play/Pause Song Action",
                     "Play or Pause the current song",
                     "This action tells the Zune Software to play or pause the current song.")
        {
        }

        public override void PerformAction(MayhemTriggerBase sender)
        {
            MayhemZune t = MayhemZune.Instance;
            t.SendCommand("^p");
        }

    }
}
