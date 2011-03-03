﻿using System;
using System.Runtime.Serialization;
using System.Windows;
using MayhemCore;

namespace DefaultModules.Reactions.Debugging
{
	[Serializable]
	public class Popup : ReactionBase
	{
		public Popup()
			: base("Debug: Popup", "Generates a small popup window when triggered") {

		}
		public override void Perform() {
			MessageBox.Show("Triggered!");
		}

		#region Serialization
		public Popup(SerializationInfo info, StreamingContext context)
			: base(info, context) {
		}

		public new void GetObjectData(SerializationInfo info, StreamingContext context) {
			base.GetObjectData(info, context);
		}
		#endregion
	}
}
