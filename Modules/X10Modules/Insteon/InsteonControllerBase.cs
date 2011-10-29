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
        public class NotInitializedException : Exception{}

        protected static MayhemSerialPortMgr mSerial = MayhemSerialPortMgr.Instance;
        protected string PortName;
        public bool Initialized;

        protected bool WaitForData; 

        // parsing
        protected byte[] ParseBuf = new byte[1024];
        //private int parse_command_len = 0;
        protected int ParseCount;

        //public bool lastCommandSuccess = false
        protected AutoResetEvent waitAck = new AutoResetEvent(false);

        protected InsteonControllerBase(string serialPortname) 
        {
            if (mSerial.ConnectPort(serialPortname, this, new InsteonUsbModemSerialSettings()))
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
