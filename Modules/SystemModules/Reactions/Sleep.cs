using System.Windows.Forms;
using MayhemCore;

namespace SystemModules.Reactions
{
    [MayhemModule("Power: Sleep", "Forces an immediate suspension to low power state")]
    public class Sleep : ReactionBase
    {
        public override void Perform()
        {
            Application.SetSuspendState(PowerState.Suspend, true, false);
        }
    }
}
