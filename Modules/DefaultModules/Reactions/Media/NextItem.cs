using System;
using System.Runtime.Serialization;
using DefaultModules.LowLevel;
using MayhemCore;

namespace DefaultModules.Reactions.Media
{
    [DataContract]
    [MayhemModule("Media: Next Item", "Goes to the next item when triggered")]
    public class NextItem : ReactionBase
    {
        public override void Perform()
        {
            Utils.SendKey((ushort)VK.MEDIA_NEXT_TRACK);
        }
    }
}
