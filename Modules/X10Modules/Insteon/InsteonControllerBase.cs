using System;
using System.Threading;
using MayhemSerial;

namespace X10Modules.Insteon
{
    /// <summary>
    /// This class manages the communication with the serial port for the Insteon Modules.
    /// It also ensures that Mayhem Modules get synchronized access to serial port messages. 
    /// </summary>
    public class InsteonControllerBase : ISerialPortDataListener, IDisposable
    {
        protected static MayhemSerialPortMgr MSerial
        {
            get;
            private set;
        }

        protected string PortName
        {
            get;
            private set;
        }

        public bool Initialized
        {
            get;
            private set;
        }

        protected bool WaitForData
        {
            get;
            set;
        }

        // parsing
        protected byte[] ParseBuf
        {
            get;
            private set;
        }

        protected int ParseCount
        {
            get;
            set;
        }

        protected AutoResetEvent waitAck
        {
            get;
            private set;
        }

        static InsteonControllerBase()
        {
            MSerial = MayhemSerialPortMgr.Instance;
        }

        protected InsteonControllerBase(string serialPortname)
        {
            ParseBuf = new byte[1024];
            waitAck = new AutoResetEvent(false);

            if (MSerial.ConnectPort(serialPortname, this, new InsteonUsbModemSerialSettings()))
            {
                PortName = serialPortname;
                InitializeX10();
                Initialized = true;
            }
        }

        private void InitializeX10()
        {
            // TODO
        }

        public virtual void Dispose()
        {
            Initialized = false;
        }

        public virtual void DataReceived(string portName, byte[] buffer, int nBytes)
        {
            throw new NotImplementedException();
        }

        public void ResetRxBuffer()
        {
            // cleanup
            ParseBuf = new byte[1024];
            ParseCount = 0;
        }
    }
}
