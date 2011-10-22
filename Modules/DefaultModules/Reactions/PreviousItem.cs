using DefaultModules.LowLevel;
using MayhemCore;

namespace DefaultModules.Reactions
{
    [MayhemModule("Media: Previous Item", "Goes to the previous item when triggered")]
    public class PreviousItem : ReactionBase
    {
        public override void Perform()
        {
            Utilities.SendKey((ushort)VK.MEDIA_PREV_TRACK);
        }
    }
}
