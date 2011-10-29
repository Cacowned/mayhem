namespace ArduinoModules.Firmata
{
    public struct FirmataMsg
    {
        public static readonly byte StartSysex = 0xF0; // start a MIDI Sysex message
        public static readonly byte EndSysex = 0xF7; // end a MIDI Sysex message
        public static readonly byte PinModeSet = 0xF4; // change pin mode
        public static readonly byte PinModeQuery = 0x72; // ask for current and supported pin modes
        public static readonly byte PinModeResponse = 0x73; // reply with current and supported pin modes
        public static readonly byte PinStateQuery = 0x6D;
        public static readonly byte PinStateResponse = 0x6E;
        public static readonly byte CapabilityQuery = 0x6B;
        public static readonly byte CapabilityResponse = 0x6C;
        public static readonly byte AnalogMappingQuery = 0x69;
        public static readonly byte AnalogMappingResponse = 0x6A;
        public static readonly byte ReportFirmware = 0x79; // report name and version of the firmware
        public static readonly byte SamplingInterval = 0x7A; // used to set the sampling interval

        // message reports
        public static readonly byte AnalogIoMessage = 0xE0;
        public static readonly byte DigitalIoMessage = 0x90;
        public static readonly byte ReportAnalogPin = 0xC0;
        public static readonly byte ReportDigitalPort = 0xD0;
    }
}
