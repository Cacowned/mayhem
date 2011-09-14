using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace X10Modules.Insteon
{
    [DataContract]
    public abstract class InsteonCommandBase
    {
        [DataMember]
        public byte[] commandBytes = null;

        


        public int length
        {
            get
            {
                if (commandBytes != null)
                {
                    return commandBytes.Length;
                }
                else
                {
                    return length;
                }
            }
        }

    }
}
