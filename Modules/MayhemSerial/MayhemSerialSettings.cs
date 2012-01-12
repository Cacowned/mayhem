using System.IO.Ports;

namespace MayhemSerial
{
    /// <summary>
    /// Settings database for various serial periphery used by Mayhem 
    /// </summary>
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
