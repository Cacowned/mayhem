using System;
using System.Runtime.Serialization;

namespace X10Modules.Insteon.InsteonCommands
{
    /// <summary>
    /// Insteon standard messages addressed to specific devices
    /// The bytes:
    /// 0x02
    /// 0x62
    /// addr-high   --> device address
    /// addr-mid
    /// addr-low
    /// flags       --> ignore for now (set to 0)
    /// c1          --> command byte 1
    /// c2          --> command byte 2
    /// </summary>
    [DataContract]
    public class InsteonStandardMessage : InsteonResponseCommand
    {
        [DataMember]
        public static readonly int StandardRespLength = 11;

        [DataMember]
        public byte CommandByte1;

        [DataMember]
        public byte CommandByte2;

        [DataMember]
        public byte[] DeviceAddress;


        /// <summary>
        /// Overload constructor with standard response length and command2 set to zero.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="c1"></param>
        public InsteonStandardMessage(byte[] address, byte c1) : this(address, c1, 0x00, StandardRespLength) { }


        /// <summary>
        /// Overload constructor with standard response length already set.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        public InsteonStandardMessage(byte[] address, byte c1, byte c2) : this(address, c1, c2, StandardRespLength) { }

        public InsteonStandardMessage(byte[] address, byte c1, byte c2, int respLength)
        {
            // if address has the wrong length throw an exception
            if (address.Length != 3)
                throw new InvalidOperationException();

            CommandBytes = new byte[]{   0x02, 0x62,                  // header for standard command
                                                address[0], address[1], address[2], // device address
                                                0x00,                               //flags (unused)
                                                c1,                                 // command byte 1
                                                c2                                  // command byte 2
                                            };

            CommandByte1 = c1;
            CommandByte2 = c2;

            DeviceAddress = address;

            ExpectedResponseLength = respLength;
        }

        /// <summary>
        /// Is this a toggle command? 
        /// </summary>
        /// <returns></returns>
        public bool IsToggleCommand
        {
            get
            {
                return CommandByte1 == InsteonCommandBytes.Toggle;
            }
        }

        /// <summary>
        /// Is this an on command?
        /// </summary>
        /// <returns></returns>
        public bool IsOnCommand
        {
            get
            {
                return CommandByte1 == InsteonCommandBytes.LightOnFast;
            }
        }

        /// <summary>
        /// Is this an off command?
        /// </summary>
        /// <returns></returns>
        public bool IsOffCommand
        {
            get
            {
                return CommandByte1 == InsteonCommandBytes.LightOffFast;
            }
        }

        public override string ToString()
        {
            if (IsToggleCommand)
                return "TOGGLE";

            if (IsOnCommand)
                return "ON";

            if (IsOffCommand)
                return "OFF";

            return string.Format("{0:X}-{1:X}", CommandByte1, CommandByte2);
        }
    }
}
