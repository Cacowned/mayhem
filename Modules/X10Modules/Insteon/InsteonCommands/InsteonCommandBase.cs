using System.Runtime.Serialization;

namespace X10Modules.Insteon.InsteonCommands
{
    [DataContract]
    public abstract class InsteonCommandBase
    {
        [DataMember]
        public byte[] CommandBytes = null;

        public int Length
        {
            get
            {
                if (CommandBytes != null)
                {
                    return CommandBytes.Length;
                }
                else
                {
                    return Length;
                }
            }
        }

    }
}
