using System;
using System.Runtime.Serialization;
using DefaultModules.LowLevel;
using MayhemCore;

namespace DefaultModules.Reactions.Media
{
    [DataContract]
    [MayhemModule("Media: Previous Item", "Goes to the previous item when triggered")]
    public class PreviousItem : ReactionBase
    {
        public PreviousItem()
        { }

        public override void Perform()
        {
            Utils.SendKey((ushort)VK.MEDIA_PREV_TRACK, 0x22);
        }
    }
}
