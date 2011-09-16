using DefaultModules.LowLevel;
using MayhemCore;

namespace DefaultModules.Reactions
{
    [MayhemModule("Media: Next Item", "Goes to the next item when triggered")]
    public class NextItem : ReactionBase
    {
        public override void Perform()
        {
            Utilities.SendKey((ushort)VK.MEDIA_NEXT_TRACK);
        }
    }
}
