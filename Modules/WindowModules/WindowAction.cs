using System;

namespace WindowModules
{
    public interface WindowAction
    {
        void Perform(IntPtr window);
    }
}
