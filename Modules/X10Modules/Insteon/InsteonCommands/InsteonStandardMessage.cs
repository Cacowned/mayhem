using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace X10Modules.Insteon
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
        public static readonly int standard_resp_length = 11;

        [DataMember]
        public byte commandByte1;

        [DataMember]
        public byte commandByte2;

        [DataMember]
        public byte[] device_address; 


        /// <summary>
        /// Overload constructor with standard response length and command2 set to zero.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="c1"></param>
        public InsteonStandardMessage(byte[] address, byte c1) : this(address, c1, 0x00, standard_resp_length) { }


        /// <summary>
        /// Overload constructor with standard response length already set.
        /// </summary>
        /// <param name="address"></param>
        /// <param name="c1"></param>
        /// <param name="c2"></param>
        public InsteonStandardMessage(byte[] address, byte c1, byte c2) : this(address, c1, c2, standard_resp_length) { }

        public InsteonStandardMessage(byte[] address, byte c1, byte c2, int resp_length)
        {
            // if address has the wrong length throw an exception
            if (address.Length != 3)
                throw new NotSupportedException();
            commandBytes = new byte[]{   (byte) 0x02, (byte) 0x62,                  // header for standard command
                                                address[0], address[1], address[2], // device address
                                                0x00,                               //flags (unused)
                                                c1,                                 // command byte 1
                                                c2                                  // command byte 2
                                            };

            commandByte1 = c1;
            commandByte2 = c2;

            device_address = address;

            this.expectedResponseLength = resp_length;
        }

        /// <summary>
        /// Is this a toggle command? 
        /// </summary>
        /// <returns></returns>
        public bool IsToggleCommand()
        {
            return commandByte1 == InsteonCommandBytes._toggle;
        }

        /// <summary>
        /// Is this an on command?
        /// </summary>
        /// <returns></returns>
        public bool IsOnCommand()
        {
            return commandByte1 == InsteonCommandBytes.light_on_fast;
        }

        /// <summary>
        /// Is this an off command?
        /// </summary>
        /// <returns></returns>
        public bool IsOffCommand()
        {
            return commandByte1 == InsteonCommandBytes.light_off_fast; 
        }



    }
}
