using System.Runtime.Serialization;

namespace X10Modules.Insteon.InsteonCommands
{
    /// <summary>
    /// Basic Insteon Command
    /// </summary>
    [DataContract]
    public class InsteonBasicCommand : InsteonCommandBase
    {
        public InsteonBasicCommand() { }

        public InsteonBasicCommand(byte[] commandString)
        {
            CommandBytes = commandString;
        }
    }
}
