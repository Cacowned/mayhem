using System;
using System.Runtime.Serialization;
using DefaultModules.LowLevel;
using MayhemCore;

namespace DefaultModules.Reactions.Media
{
    [DataContract]
    [MayhemModuleAttribute("Media: Previous Item", "Goes to the previous item when triggered")]
    public class PreviousItem : ReactionBase
    {
        public override void Perform()
        {
            Utils.SendKey((ushort)VK.MEDIA_PREV_TRACK);
        }
    }
}
