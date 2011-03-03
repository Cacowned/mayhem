using System;
using System.Runtime.Serialization;
using DefaultModules.LowLevel;
using MayhemCore;

namespace DefaultModules.Reactions.Media
{
	[Serializable]
	public class NextItem : ReactionBase
	{
		public NextItem()
			: base("Media: Next Item", "Goes to the next item when triggered") {
		}
		public override void Perform() {
			Utils.SendKey((ushort)VK.MEDIA_NEXT_TRACK);
		}

		#region Serialization
		public NextItem(SerializationInfo info, StreamingContext context)
			: base(info, context) {
		}

		public new void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);
		}
		#endregion
	}
}
