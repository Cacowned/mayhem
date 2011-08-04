using System;
using System.Runtime.Serialization;
using System.Windows;
using MayhemCore;

namespace DefaultModules.Reactions.Debugging
{
	public class Popup : ReactionBase
	{
		public Popup()
			: base("Debug: Popup", "Generates a small popup window when triggered") { }

        public override void Perform()
        {
            MessageBox.Show("Triggered!");
        }
	}
}
