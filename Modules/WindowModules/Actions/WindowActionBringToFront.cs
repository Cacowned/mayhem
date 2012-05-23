﻿using System;
using System.Runtime.Serialization;

namespace WindowModules.Actions
{
    [DataContract]
    public class WindowActionBringToFront : IWindowAction
    {
        public void Perform(IntPtr window)
        {
            Native.WINDOWPLACEMENT placement = Native.GetWindowPlacement(window);
            Native.WindowShowStyle style = (Native.WindowShowStyle)placement.showCmd;

            if (style == Native.WindowShowStyle.ShowMinimized)
                style = Native.WindowShowStyle.Restore;

            Native.ShowWindow(window, style);
            Native.SetForegroundWindow(window);

            // Another attempt to bring the window to the top.
            // Sometimes setting foreground window doesn't work
            Native.BringWindowToTop(window);
        }
    }
}
