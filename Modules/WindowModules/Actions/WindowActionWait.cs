using System;
using System.Runtime.Serialization;
using System.Threading;

namespace WindowModules.Actions
{
    [DataContract]
    public class WindowActionWait : IWindowAction
    {
        [DataMember]
        public int Milliseconds
        {
            get;
            set;
        }

        public void Perform(IntPtr window)
        {
            Thread.Sleep(Milliseconds);
        }
    }
}
