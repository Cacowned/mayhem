using System;
using System.Runtime.Serialization;

namespace WindowModules.Actions
{
    [DataContract]
    public class WindowActionMaximize : WindowAction
    {
        public void Perform(IntPtr window)
        {
            Native.ShowWindow(window, Native.WindowShowStyle.Maximize);
        }
    }
}
