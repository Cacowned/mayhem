using System.Windows.Forms;
using MayhemCore;

namespace SystemModules.Reactions
{
    [MayhemModule("Power: Hibernate", "Forces an immediate hibernate")]
    public class Hibernate : ReactionBase
    {
        public override void Perform()
        {
            Application.SetSuspendState(PowerState.Hibernate, true, false);
        }
    }
}
