using System.Collections.Generic;
using System.Runtime.Serialization;

namespace WindowModules
{
    [DataContract]
    public class WindowActionInfo
    {
        [DataMember]
        public WindowInfo WindowInfo
        {
            get;
            set;
        }

        [DataMember]
        public List<IWindowAction> WindowActions
        {
            get;
            set;
        }

        public WindowActionInfo()
        {
            WindowInfo = new WindowInfo();
            WindowActions = new List<IWindowAction>();
        }
    }
}
