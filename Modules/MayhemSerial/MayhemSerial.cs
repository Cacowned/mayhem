/*
 * MayhemSerial.cs
 * 
 * Serial Port Connection Manager for Mayhem
 * 
 * (c) 2011, Microsoft Applied Sciences Group
 * 
 * Author: Sven Kratz
 * 
 */

using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Management;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Threading;
using MayhemCore;
using Microsoft.Win32.SafeHandles;

namespace MayhemSerial
{
    public class MayhemSerialPortMgr
    {
        private readonly int verbosityLevel = 0;

        private string[] serialPortNames;
        private Dictionary<string, SerialPort> connections = new Dictionary<string, SerialPort>();

        private byte[] rxBuf = new byte[4096];

        // toggle writing to ports or not 
        private bool allowWrite = true;

        // toggle receiving or not
        private bool allowRx = true;

        public List<string> ConnectionNames
        {
            get
            {
                // return a sorted copy of tha connection keys list 
                List<string> names = connections.Keys.ToList();
                names.Sort();
                return names;
            }
        }

        // singular instance
        public static MayhemSerialPortMgr Instance = new MayhemSerialPortMgr();
        public static readonly int DwFlagsAndAttributes = 0x40000000;

        // sharing of datareceivedevent --> must write the data to a shared buffer and give a copy to the listeners
        // otherwise the modules will preemt each other in consuming the Serial read buffer

        // serialPortname, buffer, length
        public Action<string, byte[], int> OnDataReceived;

        // handlers
        private SerialDataReceivedEventHandler serialReceived;
        private SerialErrorReceivedEventHandler serialError;
        private EventHandler serialDisposed;

        // checking for presence of serial ports
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern SafeFileHandle CreateFile(string lpFileName, int dwDesiredAccess, int dwShareMode, IntPtr securityAttrs, int dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile);

        public MayhemSerialPortMgr()
        {
            serialReceived = new SerialDataReceivedEventHandler(port_DataReceived);
            serialError = new SerialErrorReceivedEventHandler(port_ErrorReceived);
            serialDisposed = new EventHandler(port_Disposed);
            UpdatePortList();
        }

        /// <summary>
        /// Updates list of port to reflect current state of the system's serial ports
        /// </summary>
        public void UpdatePortList()
        {
            serialPortNames = System.IO.Ports.SerialPort.GetPortNames();

            // check for validity of names in the list --> remove non-existant ports
            foreach (string name in ConnectionNames)
            {
                var isValid = SerialPort.GetPortNames().Any(x => string.Compare(x, name, true) == 0);
                if (!isValid)
                {
                    Logger.Write("UpdatePortList --> removing invalid name: " + name);
                    connections.Remove(name);
                }
            }

            // add new names to the list --> add new ports (i.e. plugged-in arduino) 
            foreach (string name in serialPortNames)
            {
                // -------- new name detected
                if (!ConnectionNames.Contains(name))
                {
                    SafeFileHandle hFile = CreateFile(@"\\.\" + name, -1073741824, 0, IntPtr.Zero, 3, DwFlagsAndAttributes, IntPtr.Zero);
                    if (!hFile.IsInvalid)
                    {
                        Logger.WriteLine("UpdatePortList --> adding name: " + name);
                        connections.Add(name, null);
                    }

                    hFile.Close();
                }
            }
        }

        /// <summary>
        /// Establishes opens a connection on a serial port
        /// </summary>
        /// <param name="portName">string name of the port</param>
        /// <param name="listener">reference to the data update listener</param>
        /// <param name="settings">the settings to be used for connecting</param>
        public bool ConnectPort(string portName, ISerialPortDataListener listener, SerialSettings settings)
        {
            // check if the port is known 
            if (portName != null && connections.Keys.Contains(portName))
            {
                SerialPort port = connections[portName];
                if (port == null)
                {
                    port = new SerialPort(portName,
                                           settings.BaudRate,
                                           settings.Parity,
                                           settings.DataBits,
                                           settings.StopBits);
                    port.Open();
                }

                if (port.IsOpen)
                {
                    port.DataReceived -= serialReceived;
                    port.ErrorReceived -= serialError;
                    port.Disposed -= serialDisposed;

                    port.DataReceived += serialReceived;
                    port.ErrorReceived += serialError;
                    port.Disposed += serialDisposed;

                    connections[portName] = port;

                    OnDataReceived += new Action<string, byte[], int>(listener.port_DataReceived);

                    return true;
                }
                else
                {
                    Logger.WriteLine("port did not open!");
                    port.Disposed += new EventHandler(port_Disposed);
                    port.Close();
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// Disconnect a listener from the serial port
        /// </summary>
        /// <param name="portName"></param>
        /// <param name="listener"></param>
        public void DisconnectListener(string portName, ISerialPortDataListener listener)
        {
            OnDataReceived -= listener.port_DataReceived;
        }

        public void DisconnectPort(string portName)
        {
            if (portName != null && connections.Keys.Contains(portName) && connections[portName] != null)
            {
                connections[portName].Close();
            }
        }

        /// <summary>
        /// Query the serial controller if the port exists. 
        /// Should be used on deserialization of modules using serial connections. 
        /// </summary>
        /// <param name="portName"></param>
        /// <returns>Returns existance of port name. Does not guarantee that the correct device is attached there (TODO) </returns>
        public bool PortExists(string portName)
        {
            UpdatePortList();
            return this.connections.Keys.Contains(portName);
        }

        /// <summary>
        /// Returns open state of a connected port 
        /// </summary>
        /// <param name="portName"></param>
        /// <returns></returns>
        public bool IsPortOpen(string portName)
        {
            if (connections.Keys.Contains(portName))
            {
                if (connections[portName] != null)
                {
                    return connections[portName].IsOpen;
                }
            }

            return false;
        }

        //------------------------------------
        #region sending
        [MethodImpl(MethodImplOptions.Synchronized)]
        public void WriteToPort(string portName, string s)
        {
            if (allowWrite)
            {
                if (connections.Keys.Contains(portName) && connections[portName] != null && connections[portName].IsOpen)
                {
                    SerialPort p = connections[portName];
                    lock (this)
                    {
                        p.Write(s);
                    }
                }
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void WriteToPort(string portName, byte[] buffer, int count)
        {
            Logger.WriteLine("Port: " + portName + " sending " + count + " bytes ");
            if (allowWrite)
            {
                if (connections.Keys.Contains(portName) && connections[portName] != null && connections[portName].IsOpen)
                {
                    SerialPort p = connections[portName];
                    lock (this)
                    {
                        p.Write(buffer, 0, count);
                    }
                }
            }
        }
        #endregion

        #region Query on special hardware

        /// <summary>
        /// Looks at the attached serial hardware and returns only those portnames that have an Insteon Module attached
        /// </summary>
        /// <returns>List of port name strings that correspond to an Arduino COM port</returns>
        public Dictionary<string, string> GetInsteonPortNames(SerialSettings insteonSettings)
        {
            Logger.WriteLine("getInsteonPortNames");
            UpdatePortList();

            Dictionary<string, string> pNames = new Dictionary<string, string>();

            allowRx = false;

            foreach (string portName in connections.Keys)
            {
                Logger.WriteLine("Trying (A) " + portName);

                if (connections[portName] == null)
                {
                    try
                    {
                        SerialPort port = new SerialPort(portName,
                                                insteonSettings.BaudRate,
                                                insteonSettings.Parity,
                                                insteonSettings.DataBits,
                                                insteonSettings.StopBits);

                        try
                        {
                            port.Open();
                        }
                        catch (Exception e)
                        {
                            Logger.WriteLine("Exception: " + e);
                            continue;
                        }

                        // send "Version Command to module" 
                        port.Write(new byte[] { (byte)0x02, (byte)0x69 }, 0, 2);
                        Thread.Sleep(30);
                        if (port.BytesToRead > 2)
                        {
                            byte[] bytes = new byte[2];
                            port.Read(bytes, 0, 2);
                            if (bytes[0] == 0x02 && bytes[1] == 0x69)
                            {
                                pNames.Add(portName, "Insteon Modem (" + portName + ")");
                            }
                        }

                        port.Dispose();
                    }
                    catch (ObjectDisposedException e)
                    {
                        Logger.WriteLine("ObjectDisposedException (A) " + e);
                        continue;
                    }

                    continue;
                }
                else if (connections[portName].IsOpen)
                {
                    Logger.WriteLine("Trying (B) " + portName);
                    try
                    {
                        SerialPort port = connections[portName];

                        if (port.BaudRate != 19200) continue;

                        // else: disable port data notifications for now
                        // ====================== disable receive events and sending on port during query! ===========
                        // port.DataReceived -= this.port_DataReceived;
                        allowWrite = false;

                        // =================================================
                        Thread.Sleep(10);

                        // read out remaining bytes in port buffer
                        if (port.BytesToRead > 0)
                        {
                            byte[] dummy = new byte[1024];
                            port.Read(dummy, 0, port.BytesToRead);
                        }

                        // now query
                        port.Write(new byte[] { (byte)0x02, (byte)0x69 }, 0, 2);
                        Thread.Sleep(30);
                        if (port.BytesToRead > 2)
                        {
                            byte[] bytes = new byte[2];
                            port.Read(bytes, 0, 2);
                            if (bytes[0] == 0x02 && bytes[1] == 0x69)
                            {
                                pNames.Add(portName, "Insteon Modem (" + portName + ")");
                            }
                        }

                        // ============== reenable receive envents and sending =============
                        allowWrite = true;

                        // ============================================
                    }
                    catch (ObjectDisposedException e)
                    {
                        Logger.WriteLine("ObjectDisposedException (B) " + e);
                    }
                }
            } // --------------- foreach

            allowRx = true;
            return pNames;
        }

        /// <summary>
        /// Looks at the attached serial hardware and returns only those portnames that have an Arduiono attached
        /// Works by executing a wmi query
        /// </summary>
        /// <returns>List of port name strings that correspond to an Arduino COM port</returns>
        public Dictionary<string, string> GetArduinoPortNames()
        {
            UpdatePortList();
            Dictionary<string, string> arduinoNames = new Dictionary<string, string>();
            string sInstanceName = string.Empty;
            try
            {
                ManagementObjectSearcher searcher =
                new ManagementObjectSearcher("root\\CIMV2",
                "SELECT * FROM Win32_SerialPort");

                foreach (ManagementObject queryObj in searcher.Get())
                {
                    sInstanceName = queryObj["PNPDeviceID"].ToString();

                    // TODO: Add support for further Arduino instance names
                    if (sInstanceName.IndexOf("VID_2341&PID_0001") > -1)
                    {
                        string friendlyName = queryObj["Name"].ToString();
                        string portName = queryObj["DeviceID"].ToString();
                        arduinoNames.Add(portName, friendlyName);
                        Logger.WriteLine("found Arduino UNO, Portname " + friendlyName);
                    }
                }
            }
            catch (ManagementException me)
            {
                Logger.WriteLine("managementException: " + me);
                foreach (string key in connections.Keys)
                {
                    arduinoNames.Add(key, key);
                }
            }

            return arduinoNames;
        }
        #endregion

        //--------------------------------
        #region data received and other callbacks
        /// <summary>
        /// Main Callback when new data has been received by a serial port
        /// Notifies any attached delegates of then new data in the receive buffer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Logger.WriteLineIf(verbosityLevel > 0, "----> port_DataReceived <----");
            if (allowRx)
            {
                SerialPort p = sender as SerialPort;

                // new receive buffer 
                byte[] rx = new byte[32000];
                int nBytes = p.BytesToRead;
                if (nBytes > 0 && nBytes <= rx.Length)
                {
                    p.Read(rx, 0, nBytes);
                    if (verbosityLevel > 0)
                    {
                        for (int i = 0; i < nBytes; i++)
                        {
                            Logger.Write(i + " "); 
                            Logger.WriteLine("{0,10:X}", rx[i]);
                        }
                    }
                }
                else if (nBytes > rx.Length)
                {
                    // overflow
                    p.DiscardInBuffer();
                }

                // notify any attached delegates of the received data
                if (OnDataReceived != null)
                {
                    try
                    {
                        OnDataReceived(p.PortName, rx, nBytes);
                    }
                    catch (ObjectDisposedException ex)
                    {
                        Logger.WriteLine("attempt to send data to disposed object " + ex);
                    }
                }

                // hold on to a copy of the last buffer just in case
                rxBuf = rx;
            }
            else
            {
                Logger.WriteLine("ignoring! allowRX == false");
            }
        }

        private void port_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            Logger.WriteLine("port_errorReceived " + e.ToString());
        }

        private void port_Disposed(object sender, EventArgs e)
        {
            Logger.WriteLine("port Disposed: " + e.ToString());
            SerialPort disposedPort = sender as SerialPort;
            connections.Remove(disposedPort.PortName);
        }
        #endregion
    }
}
