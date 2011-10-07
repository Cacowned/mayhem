/*
 * InsteonControllerBase.cs
 * 
 * This class manages the communication with the serial port for the Insteon Modules.
 * It also ensures that Mayhem Modules get synchronized access to serial port messages. 
 * 
 * (c) 2011, Microsoft Applied Sciences Group
 * 
 * Author: Sven Kratz
 * 
 */
using System;
using System.Threading;
using MayhemSerial;
using X10Modules.Insteon;

namespace X10Modules
{
    public  class InsteonControllerBase : ISerialPortDataListener, IDisposable
    {
        public class NotInitializedException : Exception{}

        protected static MayhemSerialPortMgr mSerial = MayhemSerialPortMgr.Instance;
        protected string portName = null;
        public bool initialized = false;
        protected bool waitForData = false; 

        // parsing
        protected byte[] parse_buf = new byte[1024];
        //private int parse_command_len = 0;
        protected int parse_count = 0;

        //public bool lastCommandSuccess = false
        protected AutoResetEvent waitAck = new AutoResetEvent(false);


        


        protected InsteonControllerBase(string serialPortname) 
        {
            if (mSerial.ConnectPort(serialPortname, this, new InsteonUsbModemSerialSettings()))
            {
                portName = ""+serialPortname;
                InitializeX10();
                this.initialized = true;
            }
        }

        private void InitializeX10()
        {
            // TODO
        }

        public virtual void Dispose()
         {
             this.initialized = false; 
         }

        public virtual void port_DataReceived(string portName, byte[] buffer, int nBytes)
        {
            throw new NotImplementedException();
        }

        public void ResetRxBuffer()
        {
            // cleanup
            parse_buf = new byte[1024];
            parse_count = 0; ;
        }
    }
}
