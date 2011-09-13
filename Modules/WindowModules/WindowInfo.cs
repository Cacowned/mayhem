using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace WindowModules
{
    [DataContract]
    public class WindowInfo
    {
        [DataMember]
        public string FileName = "";
        [DataMember]
        public string Title = "";
        [DataMember]
        public bool CheckFileName = false;
        [DataMember]
        public bool CheckTitle = false;
        //[DataMember]
        //public string ClassName = "";
    }

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
        public List<WindowAction> WindowActions
        {
            get;
            set;
        }

        public WindowActionInfo()
        {
            WindowInfo = new WindowInfo();
            WindowActions = new List<WindowAction>();
        }
    }
}
