using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace WindowModules
{
    public interface WindowAction
    {
        void Perform(IntPtr window);
    }
}
