using System.Windows;
using DefaultModules.Resources;
using MayhemCore;

namespace DefaultModules.Reactions
{
    [MayhemModule("Debug: Popup", "Generates a small popup window when triggered")]
    public class Popup : ReactionBase
    {
        public override void Perform()
        {
            MessageBox.Show(EnglishStrings.Popup_MessageText);
        }
    }
}
