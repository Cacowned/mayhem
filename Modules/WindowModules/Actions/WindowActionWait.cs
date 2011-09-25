using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Threading;

namespace WindowModules.Actions
{
    [DataContract]
    public class WindowActionWait : WindowAction
    {
        [DataMember]
        public int Milliseconds;

        public void Perform(IntPtr window)
        {
            Thread.Sleep(Milliseconds);
        }
    }
}
