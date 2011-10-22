using System.Runtime.Serialization;

namespace WindowModules
{
    [DataContract]
    public class WindowInfo
    {
        [DataMember]
        public string FileName { get; set; }

        [DataMember]
        public string Title { get; set; }

        [DataMember]
        public bool CheckFileName { get; set; }

        [DataMember]
        public bool CheckTitle { get; set; }

        public WindowInfo()
        {
            FileName = string.Empty;
            Title = string.Empty;
            CheckFileName = true;
        }
    }
}
