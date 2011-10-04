using System;
using System.Runtime.Serialization;

namespace WindowModules.Actions
{
    [DataContract]
    public class WindowActionResize : IWindowAction
    {
        [DataMember]
        public int Width;
        [DataMember]
        public int Height;

        public void Perform(IntPtr window)
        {
            Native.RECT rect = new Native.RECT();
            Native.GetWindowRect(window, ref rect);
            int x = rect.left;
            int y = rect.top;

            Native.MoveWindow(window, x, y, Width, Height, true);
        }
    }
}
