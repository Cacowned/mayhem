using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using MayhemCore;

namespace X10Modules.Insteon
{
    /// <summary>
    /// This class handles communication with an Insteon Modem in X10 Mode
    /// </summary>
    public class X10Controller : InsteonControllerBase
    {
        private static readonly Dictionary<string, X10Controller> Instances = new Dictionary<string, X10Controller>();

        private static Dictionary<X10HouseCode, Dictionary<X10UnitCode, bool>> deviceStates;

        /// <summary>
        /// Factory method for insteon controllers, so multiple events can share a controller
        /// </summary>
        /// <param name="pName"></param>
        /// <returns></returns>
        public static X10Controller ControllerForPortName(string pName)
        {
            if (!Instances.Keys.Contains(pName))
            {
                Logger.WriteLine("Creating new X10Controller for PortName: " + pName);
                Instances[pName] = new X10Controller(pName);
            }
            else
            {
                Logger.WriteLine("Returning Existing Controller");
            }

            return Instances[pName];
        }

        /// <summary>
        /// Just use base constructor
        /// </summary>
        /// <param name="portname"></param>
        private X10Controller(string portname)
            : base(portname)
        {
            // initialize device states
            if (deviceStates == null)
            {
                deviceStates = new Dictionary<X10HouseCode, Dictionary<X10UnitCode, bool>>();

                foreach (X10HouseCode hCode in Enum.GetValues(typeof(X10HouseCode)))
                {
                    deviceStates[hCode] = new Dictionary<X10UnitCode, bool>();
                    foreach (X10UnitCode uCode in Enum.GetValues(typeof(X10UnitCode)))
                    {
                        deviceStates[hCode][uCode] = false;
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool X10SendHouseCommand(X10HouseCode houseCode, X10CommandCode command)
        {
            byte[] buf = new byte[4];

            // preamble for insteon modules and tail
            buf[0] = 0x02;
            buf[1] = 0x63;
            buf[3] = 0x80;

            // do the magic for byte 2
            byte hCode = (byte)((Convert.ToByte(houseCode) & 0xf) << 4);
            byte cCode = (byte)(Convert.ToByte(command) & 0xf);
            buf[2] = (byte)(hCode | cCode);

            Logger.WriteLine("House/Command: {0,10:X} {1,10:X} {2,10:X} {3,10:X}", buf[0], buf[1], buf[2], buf[3]);
            MSerial.WriteToPort(PortName, buf, 4);
            bool wait = waitAck.WaitOne(100);

            if (wait)
            {
                Logger.WriteLine("\n=======================\nX10-ACK -- Command Successful\n====================");
                return true;
            }

            return false;
        }

        /// <summary>
        /// Chain unit and house command
        /// </summary>
        /// <param name="houseCode"></param>
        /// <param name="unitCode"></param>
        /// <param name="command"></param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool X10SendCommand(X10HouseCode houseCode, X10UnitCode unitCode, X10CommandCode command)
        {
            // set house/unit, then house/command
            byte[] buf1 = new byte[4];

            // preamble for the insteon modules
            buf1[0] = 0x02;
            buf1[1] = 0x63;

            // process toggle command
            if (command == X10CommandCode.Toggle)
            {
                if (deviceStates[houseCode][unitCode])
                {
                    command = X10CommandCode.Off;
                    deviceStates[houseCode][unitCode] = false;
                }
                else
                {
                    command = X10CommandCode.On;
                    deviceStates[houseCode][unitCode] = true;
                }
            }
            else
            {
                // set states correctly if command if on or off 
                if (command == X10CommandCode.On)
                    deviceStates[houseCode][unitCode] = true;
                else if (command == X10CommandCode.Off)
                    deviceStates[houseCode][unitCode] = false;
            }

            byte hCode = (byte)((Convert.ToByte(houseCode) & 0xf) << 4);
            byte uCode = (byte)(Convert.ToByte(unitCode) & 0xf);
            byte cCode = (byte)(Convert.ToByte(command) & 0xf);

            buf1[2] = (byte)(hCode | uCode);
            buf1[3] = 0x00; // 0x00 terminates unit selection
            Logger.WriteLine("House/Unit: {0,10:X}", buf1[2]);

            MSerial.WriteToPort(PortName, buf1, 4);
            WaitForData = true;
            bool wait = waitAck.WaitOne();
            if (wait)
            {
                Thread.Sleep(30);

                // got an ack from the port -- now send command
                Logger.WriteLine("X10-ACK");
                buf1[2] = (byte)(hCode | cCode);

                // 0x80 terminates command code
                buf1[3] = 0x80;
                Logger.WriteLine("House/Command: {0,10:X} {1,10:X} {2,10:X} {3,10:X}", buf1[0], buf1[1], buf1[2], buf1[3]);
                int retry = 10;
                WaitForData = true;
                wait = waitAck.WaitOne(100);

                while (!wait && retry-- > 0)
                {
                    MSerial.WriteToPort(PortName, buf1, 4);
                    wait = waitAck.WaitOne(100);
                    Thread.Sleep(15);
                    Logger.WriteLine("retry : " + retry);
                }

                if (wait)
                {
                    Logger.WriteLine("\n=======================\nX10-ACK -- Command Successful\n====================");
                    return true;
                }

                return false;
            }

            return false;
        }

        public override void DataReceived(string portName, byte[] buffer, int nBytes)
        {
            // check if data is from the port name we are monitoring
            if (portName == PortName && WaitForData)
            {
                if (nBytes > 0)
                {
                    // look for the ack at the end
                    if (buffer[nBytes - 1] == 0x06)
                    {
                        ParseCount = 0;
                        WaitForData = false;
                        waitAck.Set();
                    }
                }
            }
        }
    }
}
