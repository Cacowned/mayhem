using System;
using System.Runtime.Serialization;

namespace WindowModules.Actions
{
    [DataContract]
    public class WindowActionMove : IWindowAction
    {
        [DataMember]
        public int X;
        [DataMember]
        public int Y;

        public void Perform(IntPtr window)
        {
            Native.RECT rect = new Native.RECT();
            Native.GetWindowRect(window, ref rect);
            int width = rect.right - rect.left;
            int height = rect.bottom - rect.top;

            Native.MoveWindow(window, X, Y, width, height, true);
        }
    }
}
