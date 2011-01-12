using System;
using System.Runtime.Serialization;
using MayhemApp.Business_Logic.Zune;

namespace MayhemApp.Business_Logic.Actions.Zune
{
    [Serializable]
    class NextSongAction : MayhemActionBase, ISerializable
    {
        public NextSongAction(SerializationInfo info, StreamingContext context) : base(info, context) { }

        public NextSongAction(String s) : this() { }

        public NextSongAction()
            : base("Next Song Action",
                     "Goes to the next song",
                     "This action tells the Zune Software to play the next song.")
        {
        }

        public override void PerformAction(MayhemTriggerBase sender)
        {
            MayhemZune t = MayhemZune.Instance;
            t.SendCommand("^f");
        }
    }
}
