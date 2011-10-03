using System;
using System.Runtime.Serialization;

namespace WindowModules.Actions
{
    [DataContract]
    public class WindowActionResize : WindowAction
    {
        [DataMember]
        public int Width;
        [DataMember]
        public int Height;

        public void Perform(IntPtr window)
        {
            Native.RECT Rect = new Native.RECT();
            Native.GetWindowRect(window, ref Rect);
            int x = Rect.left;
            int y = Rect.top;

            Native.MoveWindow(window, x, y, Width, Height, true);
        }
    }
}
