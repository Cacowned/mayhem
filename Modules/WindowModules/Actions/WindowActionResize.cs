using System;
using System.Runtime.Serialization;

namespace WindowModules.Actions
{
    [DataContract]
    public class WindowActionResize : IWindowAction
    {
        [DataMember]
        public int Width
        {
            get;
            set;
        }

        [DataMember]
        public int Height
        {
            get;
            set;
        }

        public void Perform(IntPtr window)
        {
            var rect = new Native.RECT();
            Native.GetWindowRect(window, ref rect);
            int x = rect.left;
            int y = rect.top;

            Native.MoveWindow(window, x, y, Width, Height, true);
        }
    }
}
