using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using DefaultModules.LowLevel;

namespace DefaultModules.Reactions.Media
{
    public class PreviousItem : ReactionBase
    {
        public PreviousItem()
            : base("Media: Previous Item", "Goes to the previous item when triggered") {
        }
        public override void Perform() {
            Utils.SendKey((ushort)VK.MEDIA_PREV_TRACK);
        }
    }
}
