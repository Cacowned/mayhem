using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using MayhemCore;
using X10Modules.Insteon.InsteonCommands;

namespace X10Modules.Insteon
{
    /// <summary>
    /// Implements Insteon protocol for Mayhem
    /// </summary>
    public class InsteonController : InsteonControllerBase
    {
        private static readonly Dictionary<string, InsteonController> Instances = new Dictionary<string, InsteonController>();

        private bool linking;

        public event EventHandler OnAllLinkingCompleted;

        private InsteonCommandBase lastCommand;

        private InsteonController(string portName)
            : base(portName)
        {
        }

        /// <summary>
        /// Factory method for insteon controllers, so multiple events can share a controller
        /// </summary>
        /// <param name="pName"></param>
        /// <returns></returns>
        public static InsteonController ControllerForPortName(string pName)
        {
            if (!Instances.Keys.Contains(pName))
            {
                Logger.WriteLine("Creating new InsteonController for PortName: " + pName);
                Instances[pName] = new InsteonController(pName);
            }
            else
            {
                Logger.WriteLine("Returning Existing Controller");
            }

            return Instances[pName];
        }

        public override void DataReceived(string portName, byte[] buffer, int nBytes)
        {
            Logger.WriteLine("port_DataReceived ");

            string bytes = string.Empty;
            for (int i = 0; i < nBytes; i++)
            {
                bytes += string.Format("{0:x}-", buffer[i]);
            }

            Logger.WriteLine("Bytes: " + bytes);

            // check if data is from the port name we are monitoring
            if (portName == PortName && lastCommand != null && nBytes > 0)
            {
                if (lastCommand.GetType() == typeof(InsteonBasicCommand))
                {
                    Logger.WriteLine("Listening for Basic Command Response");
                    if (buffer[nBytes - 1] == 0x06)
                    {
                        lastCommand = null;
                        waitAck.Set();
                        return;
                    }
                }
                else if (lastCommand != null && lastCommand is InsteonResponseCommand || lastCommand is InsteonStandardMessage)
                {
                    // copy bytes to read buffer
                    for (int i = 0; i < nBytes; i++)
                    {
                        ParseBuf[i + ParseCount] = buffer[i];
                    }

                    ParseCount += nBytes;
                    InsteonResponseCommand c = lastCommand as InsteonResponseCommand;
                    Logger.WriteLine("Listing for Response Command Response");

                    // check if an ack has arrived for the last command
                    if (ParseCount >= c.Length && ParseBuf[c.Length] == 0x06)
                    {
                        // the c.length + 1 is due to expected "ACK"
                        if (ParseCount < c.Length + 1 + c.ExpectedResponseLength)
                        {
                            // wait for some more bytes to perhaps appear 
                            return;
                        }

                        if (ParseCount == c.Length + 1 + c.ExpectedResponseLength)
                        {
                            // see if all-linking completed has been received
                            if (ParseBuf[1] == InsteonCommandBytes.AllLinkingCompleted)
                            {
                                Logger.WriteLine("All-Link Completed");
                                if (OnAllLinkingCompleted != null)
                                    OnAllLinkingCompleted(this, new EventArgs());
                            }

                            waitAck.Set();
                            return;
                        }

                        // gibberish
                        Logger.WriteLine("Response Length was not as expected");
                        Logger.WriteLine("Expected " + (c.ExpectedResponseLength + c.Length) + " got " + ParseCount);

                        // cleanup
                        ResetRxBuffer();
                        return;
                    }
                }
                else
                {
                    Logger.WriteLine("listening for unknown response type");
                    throw new NotSupportedException();
                }
            }
        }

        public override void Dispose()
        {
            MSerial.DisconnectListener(PortName, this);
            base.Dispose();
        }

        #region Sending Insteon Commands

        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool SendStandardMsg(InsteonStandardMessage c)
        {
            lastCommand = c;

            ////////// logic for toggle support
            if (c.IsToggleCommand)
            {
                bool device_state = InsteonDevice.GetPowerStateForDeviceAddress(c.DeviceAddress);
                if (device_state)
                {
                    return SendStandardMsg(new InsteonStandardMessage(c.DeviceAddress, InsteonCommandBytes.LightOffFast, 0, 11));
                }

                return SendStandardMsg(new InsteonStandardMessage(c.DeviceAddress, InsteonCommandBytes.LightOnFast, 0, 11));
            }

            if (c.IsOnCommand)
            {
                InsteonDevice.SetPowerStateForDeviceAddress(c.DeviceAddress, true);
            }
            else if (c.IsOffCommand)
            {
                InsteonDevice.SetPowerStateForDeviceAddress(c.DeviceAddress, false);
            }
            /////////

            MSerial.WriteToPort(PortName, c.CommandBytes, c.Length);
            bool wait = waitAck.WaitOne(100);
            if (wait)
            {
                Logger.WriteLine("\n=========\nInsteon Command " + c + " Successful\n==========\n");
                lastCommand = null;
                ResetRxBuffer();
                return true;
            }

            Logger.WriteLine("Command " + c + " unsuccessful :(");
            lastCommand = null;
            ResetRxBuffer();
            return false;
        }

        /// <summary>
        /// Turn device off
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool SendOff(InsteonDevice d)
        {
            InsteonStandardMessage c = new InsteonStandardMessage(d.DeviceId, InsteonCommandBytes.LightOffFast, 0, 11);
            return SendStandardMsg(c);
        }

        /// <summary>
        /// Turn device on
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool SendOn(InsteonDevice d)
        {
            InsteonStandardMessage c = new InsteonStandardMessage(d.DeviceId, InsteonCommandBytes.LightOnFast, 0, 11);
            return SendStandardMsg(c);
        }

        /// <summary>
        /// Enumerate devices on Insteon bus
        /// </summary>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public List<InsteonDevice> EnumerateLinkedDevices()
        {
            Logger.WriteLine("========================== EnumerateLinkedDevices===========================");
            List<InsteonDevice> devices = new List<InsteonDevice>();

            InsteonResponseCommand c = new InsteonResponseCommand(InsteonCommandBytes.GetFirstLinkRecord, 10);
            lastCommand = c;
            ResetRxBuffer();
            MSerial.WriteToPort(PortName, c.CommandBytes, c.Length);

            bool wait = waitAck.WaitOne(100);
            if (wait)
            {
                Logger.WriteLine("\n=========\nInsteon Command Successful\n==========\n");

                InsteonDevice d = new InsteonDevice();

                byte[] idBytes = new byte[3];
                Array.Copy(ParseBuf, lastCommand.Length + 5, idBytes, 0, 3);
                d.DeviceId = idBytes;
                d.AlRecordFlags = ParseBuf[lastCommand.Length + 3];
                d.ALGroup = ParseBuf[lastCommand.Length + 4];

                byte[] linkData = new byte[3];
                Array.Copy(ParseBuf, lastCommand.Length + 8, linkData, 0, 3);
                d.LinkData = linkData;
                devices.Add(d);
                Logger.WriteLine("Added device: " + d);

                // see if we have more devices in the list 
                ResetRxBuffer();
                while (wait)
                {
                    c = new InsteonResponseCommand(InsteonCommandBytes.GetNextLinkRecord, 10);
                    lastCommand = c;

                    MSerial.WriteToPort(PortName, c.CommandBytes, c.Length);
                    wait = waitAck.WaitOne(100);
                    if (!wait)
                    {
                        Logger.WriteLine("last device or invalid response");
                        break;
                    }

                    d = new InsteonDevice();

                    idBytes = new byte[3];
                    Array.Copy(ParseBuf, lastCommand.Length + 5, idBytes, 0, 3);
                    d.DeviceId = idBytes;
                    d.AlRecordFlags = ParseBuf[lastCommand.Length + 3];
                    d.ALGroup = ParseBuf[lastCommand.Length + 4];

                    linkData = new byte[3];
                    Array.Copy(ParseBuf, lastCommand.Length + 8, linkData, 0, 3);
                    d.LinkData = linkData;
                    devices.Add(d);
                    Logger.WriteLine("Added device: " + d);

                    // see if we have more devices in the list 
                    ResetRxBuffer();
                }

                ResetRxBuffer();
                lastCommand = null;
                return devices;
            }

            Logger.WriteLine("\n=========\nInsteon Command FAILED\n==========\n");

            // cleanup
            ResetRxBuffer();
            lastCommand = null;
            return null;
        }

        /// <summary>
        /// Starts the Insteon's linking mode, that allows other devices to be connected
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool StartAllLinking()
        {
            if (!linking)
            {
                linking = true;
                InsteonBasicCommand c = new InsteonBasicCommand(InsteonCommandBytes.StartAllLinking);
                lastCommand = c;
                MSerial.WriteToPort(PortName, c.CommandBytes, c.Length);
                bool wait = waitAck.WaitOne(100);
                if (wait)
                {
                    Logger.WriteLine("\n=========\nInsteon Command Successful\n==========\n");
                    return true;
                }

                return false;
            }

            return false;
        }

        /// <summary>
        /// Stops all linking mode (++++++++TODO++++++++++)
        /// </summary>    
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void stopAllLinking()
        {
            if (linking)
            {
                InsteonBasicCommand c = new InsteonBasicCommand(InsteonCommandBytes.CancelAllLinking);
                lastCommand = c;
                MSerial.WriteToPort(PortName, c.CommandBytes, c.Length);
                bool wait = waitAck.WaitOne(100);
                if (wait)
                {
                    Logger.WriteLine("\n=========\nInsteon Command Successful\n==========\n");
                }
                else
                {
                    Logger.WriteLine("\n ======= \n Command not Successful\n ============\n");
                }
            }
            else
            {
                Logger.WriteLine("\n ======= \n Command not Successful\n ============\n");
            }
        }
        #endregion
    }
}