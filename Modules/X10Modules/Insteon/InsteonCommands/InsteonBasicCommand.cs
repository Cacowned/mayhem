using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace X10Modules.Insteon
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
            commandBytes = commandString;
        }


    }
}
