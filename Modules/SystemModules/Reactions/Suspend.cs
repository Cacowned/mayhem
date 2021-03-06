﻿using System.Windows.Forms;
using MayhemCore;

namespace SystemModules.Reactions
{
    [MayhemModule("Suspend", "Forces an immediate suspension to low power state")]
    public class Suspend : ReactionBase
    {
        public override void Perform()
        {
            Application.SetSuspendState(PowerState.Suspend, true, false);
        }
    }
}
