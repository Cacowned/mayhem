namespace X10Modules.Insteon
{
    public enum X10CommandCode
    {
        ALL_LIGHTS_OFF = 0x6,
        STATUS_OFF = 0xE,
        ON = 0x2,
        PRE_SET_DIM_A = 0xA,
        ALL_LIGHTS_ON = 0x1,
        HAIL_ACK = 0x9,
        BRIGHT = 0x5,
        STATUS_ON = 0xD,
        EXTENDED_CODE = 0x7,
        STATUS_REQ = 0xF,
        OFF = 0x3,
        PRE_SET_DIM_B = 0xB,
        ALL_UNITS_OFF = 0x0,
        HAIL_REQ = 0x8,
        DIM = 0x4,
        EXTENDED_DATA = 0xC,
        TOGGLE = 0xf0      // Mayhem use! Not an actual X10 Command. 
    }
}
