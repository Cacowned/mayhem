using System;
using System.Windows;
using System.Windows.Threading;
using DebugModules.Resources;
using MayhemCore;

namespace DebugModules.Reactions
{
    [MayhemModule("Debug: Popup", "Generates a small popup window when triggered")]
    public class Popup : ReactionBase
    {
        public override void Perform()
        {
            // Use the dispatcher to bring the message box to the top
            Dispatcher.CurrentDispatcher.Invoke((Action)(OnAction));          
        }

        private void OnAction()
        {
            MessageBox.Show(Strings.Popup_MessageText);
        }
    }
}
