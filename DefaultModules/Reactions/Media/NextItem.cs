using DefaultModules.LowLevel;
using MayhemCore;
using System;

namespace DefaultModules.Reactions.Media
{
    [Serializable]
    public class NextItem: ReactionBase
    {
        public NextItem()
            :base("Media: Next Item", "Goes to the next item when triggered")
        {
        }
        public override void Perform()
        {
            Utils.SendKey((ushort)VK.MEDIA_NEXT_TRACK);
        }
    }
}
