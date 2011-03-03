﻿using System;
using System.Runtime.Serialization;
using DefaultModules.LowLevel;
using MayhemCore;

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
			: base(info, context) {
		}

		public new void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);
		}
		#endregion
	}
}