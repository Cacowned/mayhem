using System;
using System.Runtime.Serialization;
using System.Windows;
using MayhemCore;

namespace DefaultModules.Reactions.Debugging
{
    [MayhemModule("Debug: Popup", "Generates a small popup window when triggered")]
    public class Popup : ReactionBase
	{
        public Popup()
        { }

        public override void Perform()
        {
            MessageBox.Show("Triggered!");
        }
	}
}
