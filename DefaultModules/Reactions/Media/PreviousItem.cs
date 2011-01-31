using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using DefaultModules.LowLevel;
using System.Runtime.Serialization;

namespace DefaultModules.Reactions.Media
{
    [Serializable]
    public class PreviousItem : ReactionBase
    {
        public PreviousItem()
            : base("Media: Previous Item", "Goes to the previous item when triggered") {
        }
        public override void Perform() {
            Utils.SendKey((ushort)VK.MEDIA_PREV_TRACK);
        }

        #region Serialization
        public PreviousItem(SerializationInfo info, StreamingContext context) 
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
