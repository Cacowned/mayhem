/*
 * InsteonController.cs
 * 
 * Implements Insteon protocol for Mayhem
 * 
 * (c) 2011, Microsoft Applied Sciences Group
 * 
 * 
 * Author: Sven Kratz
 * 
 */ 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using MayhemCore;

namespace X10Modules.Insteon
{
    

    public class InsteonController : InsteonControllerBase
    {
        private InsteonController(string portName) : base(portName){}

        private static Dictionary<string, InsteonController> instances = new Dictionary<string, InsteonController>();

        /// <summary>
        /// Factory method for insteon controllers, so multiple events can share a controller
        /// </summary>
        /// <param name="pName"></param>
        /// <returns></returns>
        public static InsteonController ControllerForPortName(string pName)
        {
            if (!instances.Keys.Contains(pName))
            {
                Logger.WriteLine("Creating new InsteonController for PortName: " + pName);
                instances[pName] = new InsteonController(pName);
            }
            else
            {
                Logger.WriteLine("Returning Existing Controller");
            }
            return instances[pName];
        }

        private bool linking = false;

        public event EventHandler OnAllLinkingCompleted; 

        public InsteonCommandBase lastCommand = null;

        public override void port_DataReceived(string portName, byte[] buffer, int nBytes)
        {
            Logger.WriteLine("port_DataReceived ");

            string bytes = "";
            for (int i = 0; i < nBytes; i++ )
            {
                bytes += string.Format("{0:x}-", buffer[i]);
            }
            Logger.WriteLine("Bytes: " + bytes);

           
            // check if data is from the port name we are monitoring
            if (portName == this.portName && lastCommand != null && nBytes > 0)
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
                else if (lastCommand != null && lastCommand is InsteonResponseCommand  || lastCommand is InsteonStandardMessage)
                {
                    // copy bytes to read buffer
                    for (int i = 0; i < nBytes; i++)
                    {
                        parse_buf[i + parse_count] = buffer[i];
                      
                    }
                    parse_count += nBytes;                   
                    InsteonResponseCommand c = lastCommand as InsteonResponseCommand;
                    Logger.WriteLine("Listing for Response Command Response");
                    // check if an ack has arrived for the last command
                    if (parse_count >= c.length && parse_buf[c.length] == 0x06)
                    {
                        // the c.length + 1 is due to expected "ACK"
                        if (parse_count < c.length+1 + c.expectedResponseLength)
                        {
                            // wait for some more bytes to perhaps appear 
                            return;
                        }
                        if (parse_count == c.length+1 + c.expectedResponseLength)
                        {
                            // see if all-linking completed has been received
                            if (parse_buf[1] == InsteonCommandBytes.all_linking_completed)
                            {
                                Logger.WriteLine("All-Link Completed");
                                if (OnAllLinkingCompleted != null)
                                    OnAllLinkingCompleted(this, new EventArgs());
                            }

                            waitAck.Set();
                            return;
                        }
                        else
                        {
                            // gibberish
                            Logger.WriteLine("Response Length was not as expected");
                            Logger.WriteLine("Expected " + (c.expectedResponseLength+c.length) + " got " + parse_count);
                      
                            // cleanup
                            ResetRxBuffer();
                            return;
                        }
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
            mSerial.DisconnectListener(portName, this);
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
                bool device_state = InsteonDevice.GetPowerStateForDeviceAddress(c.device_address);
                if (device_state == true)
                {
                    return SendStandardMsg(new InsteonStandardMessage(c.device_address, InsteonCommandBytes.light_off_fast, 0, 11));
                }
                else
                {
                    return SendStandardMsg(new InsteonStandardMessage(c.device_address, InsteonCommandBytes.light_on_fast, 0, 11));
                }
            }
            else if (c.IsOnCommand)
            {
                InsteonDevice.SetPowerStateForDeviceAddress(c.device_address, true);
            }
            else if (c.IsOffCommand)
            {
                InsteonDevice.SetPowerStateForDeviceAddress(c.device_address, false);
            }
            /////////

            mSerial.WriteToPort(portName, c.commandBytes, c.length);
            bool wait = waitAck.WaitOne(100);
            if (wait)
            {
                Logger.WriteLine("\n=========\nInsteon Command " + c + " Successful\n==========\n");
                lastCommand = null;
                ResetRxBuffer();
                return true;
            }
            else
            {
                Logger.WriteLine("Command " + c + " unsuccessful :(");
                lastCommand = null; 
                ResetRxBuffer();
                return false;
            }
        }

        /// <summary>
        /// Turn device off
        /// </summary>
        /// <param name="d"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool SendOff(InsteonDevice d)
        {
            InsteonStandardMessage c = new InsteonStandardMessage(d.deviceID, InsteonCommandBytes.light_off_fast, 0, 11);
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
            InsteonStandardMessage c = new InsteonStandardMessage(d.deviceID, InsteonCommandBytes.light_on_fast, 0, 11);
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

            InsteonResponseCommand c = new InsteonResponseCommand(InsteonCommandBytes.get_first_link_record, 10);
            this.lastCommand = c;
            ResetRxBuffer();
            mSerial.WriteToPort(portName, c.commandBytes, c.length);

            bool wait = waitAck.WaitOne(100);
            if (wait)
            {
                Logger.WriteLine("\n=========\nInsteon Command Successful\n==========\n");

                InsteonDevice d = new InsteonDevice();

                byte[] idBytes = new byte[3];
                Array.Copy(parse_buf, lastCommand.length + 5, idBytes, 0, 3);
                d.deviceID  = idBytes;
                d.ALRecordFlags = parse_buf[lastCommand.length + 3];
                d.ALGroup = parse_buf[lastCommand.length + 4];

                byte[] linkData = new byte[3];
                Array.Copy(parse_buf, lastCommand.length + 8, linkData, 0, 3);
                d.linkData = linkData;
                devices.Add(d);
                Logger.WriteLine("Added device: " + d.ToString());

                // see if we have more devices in the list 
                ResetRxBuffer();
                while (wait)
                {
                    c = new InsteonResponseCommand(InsteonCommandBytes.get_next_link_record, 10);
                    lastCommand = c;

                    mSerial.WriteToPort(portName, c.commandBytes, c.length);
                    wait = waitAck.WaitOne(100);
                    if (!wait)
                    {
                        Logger.WriteLine("last device or invalid response");
                        break;
                    }

                    d = new InsteonDevice();

                    idBytes = new byte[3];
                    Array.Copy(parse_buf, lastCommand.length + 5, idBytes, 0, 3);
                    d.deviceID = idBytes;
                    d.ALRecordFlags = parse_buf[lastCommand.length + 3];
                    d.ALGroup = parse_buf[lastCommand.length + 4];

                    linkData = new byte[3];
                    Array.Copy(parse_buf, lastCommand.length + 8, linkData, 0, 3);
                    d.linkData = linkData;
                    devices.Add(d);
                    Logger.WriteLine("Added device: " + d.ToString());

                    // see if we have more devices in the list 
                    ResetRxBuffer();
                }

                ResetRxBuffer();
                lastCommand = null;
                return devices;
            }
            else
            {
                Logger.WriteLine("\n=========\nInsteon Command FAILED\n==========\n");
                // cleanup
                ResetRxBuffer();
                lastCommand = null;
                return null; 
            }
            
        }

        /// <summary>
        /// Starts the Insteon's linking mode, that allows other devices to be connected
        /// </summary>
        [MethodImpl(MethodImplOptions.Synchronized)]
        public bool startAllLinking()
        {
            if (!linking)
            {
                linking = true; 
                InsteonBasicCommand c = new InsteonBasicCommand(InsteonCommandBytes.start_all_linking);
                lastCommand = c;
                mSerial.WriteToPort(portName, c.commandBytes, c.length);
                bool wait = waitAck.WaitOne(100);
                if (wait)
                {
                    Logger.WriteLine("\n=========\nInsteon Command Successful\n==========\n");
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false; 
            }
        }

        /// <summary>
        /// Stops all linking mode (++++++++TODO++++++++++)
        /// </summary>    
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void stopAllLinking()
        {

            if (linking)
            {
                InsteonBasicCommand c = new InsteonBasicCommand(InsteonCommandBytes.cancel_all_linking);
                lastCommand = c;
                mSerial.WriteToPort(portName, c.commandBytes, c.length);
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
