using DefaultModules.LowLevel;
using MayhemCore;
using System;

namespace DefaultModules.Reactions.Media
{
    [Serializable]
    public class PlayPause : ReactionBase
    {
        public PlayPause()
            : base("Media: Play / Pause", "Plays or pauses the current item when triggered") {
        }
        public override void Perform() {
            Utils.SendKey((ushort)VK.MEDIA_PLAY_PAUSE);
        }
    }
}
