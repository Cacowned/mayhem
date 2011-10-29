using System.Runtime.Serialization;

namespace X10Modules.Insteon.InsteonCommands
{
    /// <summary>
    /// Insteon command that expects a response
    /// </summary>
    [DataContract]
    public class InsteonResponseCommand : InsteonBasicCommand
    {
        [DataMember]
        public int ExpectedResponseLength;

        public InsteonResponseCommand() { }

        public InsteonResponseCommand(byte[] commandBytes, int respLength)
            : base(commandBytes)
        {
            ExpectedResponseLength = respLength;
        }
    }
}
