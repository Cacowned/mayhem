using System;

namespace WindowModules
{
    public interface IWindowAction
    {
        void Perform(IntPtr window);
    }
}
