using System;
using System.Runtime.Serialization;

namespace WindowModules.Actions
{
    [DataContract]
    public class WindowActionClose : WindowAction
    {
        public void Perform(IntPtr window)
        {
            Native.PostMessage(window, Native.WM_SYSCOMMAND, Native.SC_CLOSE, 0);
        }
    }
}
