namespace X10Modules.Insteon
{
    public enum X10CommandCode
    {
        AllLightsOff = 0x6,
        StatusOff = 0xE,
        On = 0x2,
        PreSetDimA = 0xA,
        AllLightsOn = 0x1,
        HailAck = 0x9,
        Bright = 0x5,
        StatusOn = 0xD,
        ExtendedCode = 0x7,
        StatusReq = 0xF,
        Off = 0x3,
        PreSetDimB = 0xB,
        AllUnitsOff = 0x0,
        HailReq = 0x8,
        Dim = 0x4,
        ExtendedData = 0xC,
        Toggle = 0xf0      // Mayhem use! Not an actual X10 Command. 
    }
}
