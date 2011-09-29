using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MayhemCore
{
    public class EnablingEventArgs : EventArgs
    {
        public bool Cancel = false;
        public bool IsConfiguring
        {
            get;
            private set;
        }

        public EnablingEventArgs(bool isConfiguring)
        {
            this.IsConfiguring = isConfiguring;
        }
    }
}
