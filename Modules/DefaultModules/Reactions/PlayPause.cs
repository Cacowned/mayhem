﻿using DefaultModules.LowLevel;
using MayhemCore;

namespace DefaultModules.Reactions
{
    [MayhemModule("Media: Play / Pause", "Plays or pauses the current item when triggered")]
    public class PlayPause : ReactionBase
    {
        public override void Perform()
        {
            Utilities.SendKey((ushort)VK.MEDIA_PLAY_PAUSE);
        }
    }
}
