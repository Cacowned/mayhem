using DefaultModules.LowLevel;
using MayhemCore;
using System;
using System.Runtime.Serialization;

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

        #region Serialization
        public PlayPause(SerializationInfo info, StreamingContext context) 
            : base (info, context)
        {
        }

        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
        #endregion
    }
}
