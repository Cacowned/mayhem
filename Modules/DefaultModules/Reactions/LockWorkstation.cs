using System.Runtime.InteropServices;
using MayhemCore;

namespace DefaultModules.Reactions
{
    [MayhemModule("Lock Computer", "Locks the computer")]
    public class LockMachine : ReactionBase
    {
        [DllImport("user32.dll")]
        public static extern void LockWorkStation();

        public override void Perform()
        {
            LockWorkStation();
        }
    }
}
