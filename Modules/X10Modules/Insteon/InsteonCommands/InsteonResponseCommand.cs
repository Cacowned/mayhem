using System.Runtime.Serialization;

namespace X10Modules.Insteon
{
    /// <summary>
    /// Insteon command that expects a response
    /// </summary>
    [DataContract]
    public class InsteonResponseCommand : InsteonBasicCommand
    {
        [DataMember]
        public int expectedResponseLength = 0;

        public InsteonResponseCommand() { }

        public InsteonResponseCommand(byte[] commandBytes, int resp_length)
            : base(commandBytes)
        {
            expectedResponseLength = resp_length;
        }
    }
}
