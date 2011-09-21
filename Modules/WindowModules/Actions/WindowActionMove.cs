using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace WindowModules.Actions
{
    [DataContract]
    public class WindowActionMove : WindowAction
    {
        [DataMember]
        public int X;
        [DataMember]
        public int Y;

        public void Perform(IntPtr window)
        {
            Native.RECT Rect = new Native.RECT();
            Native.GetWindowRect(window, ref Rect);
            int width = Rect.right - Rect.left;
            int height = Rect.bottom - Rect.top;

            Native.MoveWindow(window, X, Y, width, height, true);
        }
    }
}
