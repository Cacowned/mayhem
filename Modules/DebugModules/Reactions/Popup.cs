using System.Windows;
using DebugModules.Resources;
using MayhemCore;

namespace DebugModules.Reactions
{
    [MayhemModule("Debug: Popup", "Generates a small popup window when triggered")]
    public class Popup : ReactionBase
    {
        public override void Perform()
        {
            MessageBox.Show(Strings.Popup_MessageText);
        }
    }
}
