namespace X10Modules.Insteon
{
    /// <summary>
    /// Stores command codes for Insteon Modem
    /// </summary>
    static class InsteonCommandBytes
    {
        // ----------------------------- Insteon Modem
        public static byte[] StartAllLinking = new byte[] { (byte)0x02, (byte)0x64, (byte)0x01, (byte)0x01 };     // tells modem to enter all-link mode
        public static byte[] CancelAllLinking = new byte[] { (byte)0x02, (byte)0x65 };                              // tells modem to cancel all-link mode
        public static byte[] GetFirstLinkRecord = new byte[] {(byte) 0x02, (byte) 0x69 };                        // get first device from records
        public static byte[] GetNextLinkRecord = new byte[] { (byte)0x02, (byte) 0x6A };                          // get next device
                                                                                                                    // if no further devices in records: modem will send NACK (0x15) 
        public static byte[] Version = new byte[] { (byte)0x02, (byte) 0x69 };

        // ----------------------------------

        // ------------------------ Insteon Standard Messages
        public static byte LightOnFast = (byte) 0x12;         // turns light on
        public static byte LightOffFast = (byte) 0x14;        // turns light off
        // ----------------------------------

        // ------------------------- Command codes RXed by host device

        public static byte AllLinkingCompleted = (byte)0x53;          // all-linking was successful

        //---------------------------------------------


        // ------------------------ Special "Byte" used by Mayhem 
        public static byte Toggle = (byte)0xf0; 


    }

   

   

    

   

    
}
