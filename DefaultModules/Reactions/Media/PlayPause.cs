using System;
using System.Runtime.Serialization;
using DefaultModules.LowLevel;
using MayhemCore;

namespace DefaultModules.Reactions.Media
{
	public class PlayPause : ReactionBase
	{
		public PlayPause()
			: base("Media: Play / Pause", "Plays or pauses the current item when triggered") 
        {
		}

		public override void Perform() 
        {
			Utils.SendKey((ushort)VK.MEDIA_PLAY_PAUSE, 0x22);
		}
	}
}
