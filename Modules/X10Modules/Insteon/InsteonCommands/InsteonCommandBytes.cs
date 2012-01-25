namespace X10Modules.Insteon.InsteonCommands
{
    /// <summary>
    /// Stores command codes for Insteon Modem
    /// </summary>
    internal static class InsteonCommandBytes
    {
        // ----------------------------- Insteon Modem
        // tells modem to enter all-link mode
        public static byte[] StartAllLinking
        {
            get;
            private set;
        }

        // tells modem to cancel all-link mode
        public static byte[] CancelAllLinking
        {
            get;
            private set;
        }

        // get first device from records        
        public static byte[] GetFirstLinkRecord
        {
            get;
            private set;
        }

        // get next device
        public static byte[] GetNextLinkRecord
        {
            get;
            private set;
        }

        // if no further devices in records: modem will send NACK (0x15) 
        public static byte[] Version
        {
            get;
            private set;
        }

        // ------------------------ Insteon Standard Messages
        // turns light on
        public static byte LightOnFast
        {
            get;
            private set;
        }

        // turns light off
        public static byte LightOffFast
        {
            get;
            private set;
        }

        // ------------------------- Command codes RXed by host device
        // all-linking was successful
        public static byte AllLinkingCompleted
        {
            get;
            private set;
        }

        // ------------------------ Special "Byte" used by Mayhem 
        public static byte Toggle
        {
            get;
            private set;
        }

        static InsteonCommandBytes()
        {
            StartAllLinking = new byte[] { 0x02, 0x64, 0x01, 0x01 };
            CancelAllLinking = new byte[] { 0x02, 0x65 };
            GetFirstLinkRecord = new byte[] { 0x02, 0x69 };
            GetNextLinkRecord = new byte[] { 0x02, 0x6A };
            Version = new byte[] { 0x02, 0x69 };
            LightOnFast = 0x12;
            LightOffFast = 0x14;
            AllLinkingCompleted = 0x53;

            Toggle = 0xf0;
        }
    }
}
