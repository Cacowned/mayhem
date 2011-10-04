using System.Collections.Generic;
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
        public bool CheckFileName = true;
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
