using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Serialization;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using DefaultModules.Wpf;
using System.IO;
using System.Runtime.InteropServices;

namespace DefaultModules.Reactions
{
    [MayhemModule("Lock Computer", "Lock the computer")]
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
