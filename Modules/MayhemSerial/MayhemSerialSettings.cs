/*
 * MayhemSerialSettings.cs
 * 
 * Settings database for various serial periphery used by Mayhem 
 * 
 * (c) 2011, Microsoft Applied Sciences Group
 * 
 * Author: Sven Kratz
 */
using System.IO.Ports;

namespace MayhemSerial
{
    public abstract class SerialSettings
    {
        public abstract int BaudRate
        {
            get;
        }

        public abstract Parity Parity
        {
            get;
        }

        public abstract StopBits StopBits
        {
            get;
        }

        public abstract int DataBits
        {
            get;
        }
    }
}
