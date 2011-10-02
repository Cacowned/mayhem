/*
 * InsteonCommands.cs
 * 
 * Stores command strings for Insteon Modem device
 * 
 * (c) 2011, Microsoft Applied Sciences Group
 * 
 * Author: Sven Kratz
 * 
 */

namespace X10Modules.Insteon
{
    /// <summary>
    /// Stores command codes for Insteon Modem
    /// </summary>
    static class InsteonCommandBytes
    {
        // ----------------------------- Insteon Modem
        public static byte[] start_all_linking = new byte[] { (byte)0x02, (byte)0x64, (byte)0x01, (byte)0x01 };     // tells modem to enter all-link mode
        public static byte[] cancel_all_linking = new byte[] { (byte)0x02, (byte)0x65 };                              // tells modem to cancel all-link mode
        public static byte[] get_first_link_record = new byte[] {(byte) 0x02, (byte) 0x69 };                        // get first device from records
        public static byte[] get_next_link_record = new byte[] { (byte)0x02, (byte) 0x6A };                          // get next device
                                                                                                                    // if no further devices in records: modem will send NACK (0x15) 
        public static byte[] version = new byte[] { (byte)0x02, (byte) 0x69 };

        // ----------------------------------

        // ------------------------ Insteon Standard Messages
        public static byte light_on_fast = (byte) 0x12;         // turns light on
        public static byte light_off_fast = (byte) 0x14;        // turns light off
        // ----------------------------------

        // ------------------------- Command codes RXed by host device

        public static byte all_linking_completed = (byte)0x53;          // all-linking was successful

        //---------------------------------------------


        // ------------------------ Special "Byte" used by Mayhem 
        public static byte _toggle = (byte)0xf0; 


    }

   

   

    

   

    
}
