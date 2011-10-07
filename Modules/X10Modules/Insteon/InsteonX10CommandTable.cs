/*
 * InsteonX10CommandTable.cs
 * 
 * This file contains enums of the X10 command table used by the 10 modules
 * 
 * (c) 2011, Microsoft Applied Sciences Group
 * 
 * Author: Sven Kratz
 * 
 */



namespace X10Modules.Insteon
{
    public enum  X10HouseCode
    {
        A = 0x6,
        B = 0xE,
        C = 0x2,
        D = 0xA,
        E = 0x1,
        F = 0x9,
        G = 0x5,
        H = 0xD,
        I = 0x7,
        J = 0xF,
        K = 0x3,
        L = 0xB,
        M = 0x0,
        N = 0x8,
        O = 0x4,
        P = 0xC
    }

    public enum X10UnitCode
    {
        U1 = 0x6,
        U2 = 0xE,
        U3 = 0x2,
        U4 = 0xA,
        U5 = 0x1,
        U6 = 0x9,
        U7 = 0x5,
        U8 = 0xD,
        U9 = 0x7,
        U10 = 0xF,
        U11 = 0x3,
        U12 = 0xB,
        U13 = 0x0,
        U14 = 0x8,
        U15 = 0x4,
        U16 = 0xC
    }

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
