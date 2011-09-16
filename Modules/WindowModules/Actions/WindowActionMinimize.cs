﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace WindowModules.Actions
{
    [DataContract]
    public class WindowActionMinimize : WindowAction
    {
        public void Perform(IntPtr window)
        {
            Native.ShowWindow(window, Native.WindowShowStyle.Minimize);
        }
    }
}
