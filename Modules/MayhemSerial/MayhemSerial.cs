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
using System.Linq;
using System.Text;
using System.IO.Ports;
using System.Runtime.CompilerServices;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;
using System.Management;
using System.Collections;
using Microsoft.Win32;
using System.Threading;

namespace MayhemSerial
{
  
    public class MayhemSerialPortMgr
    {
        public static readonly string TAG = "[MayhemSerial] : ";
        public string[] serialPortNames; 
        private Dictionary<string, SerialPort> connections = new Dictionary<string, SerialPort>();

        private byte[] rxBuf = new byte[4096];

        // toggle writing to ports or not 
        private bool allowWrite = true; 

        // toggle receiving or not
        private bool allowRX = true; 

        public List<string> connectionNames
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
        public static MayhemSerialPortMgr instance = new MayhemSerialPortMgr();
        public static readonly int dwFlagsAndAttributes = 0x40000000;

        // sharing of datareceivedevent --> must write the data to a shared buffer and give a copy to the listeners
        // otherwise the modules will preemt each other in consuming the Serial read buffer

        // serialPortname, buffer, length
        public Action<string, byte[], int> OnDataReceived;  


        // handlers

        SerialDataReceivedEventHandler serial_received; 
        SerialErrorReceivedEventHandler serial_error;
        EventHandler serial_disposed;

        private readonly int VERBOSITY_LEVEL = 0; 

        

        //checking for presence of serial ports
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern SafeFileHandle CreateFile(string lpFileName, int dwDesiredAccess, int dwShareMode, IntPtr securityAttrs, int dwCreationDisposition, int dwFlagsAndAttributes, IntPtr hTemplateFile);


        public MayhemSerialPortMgr()
        {
           serial_received =  new SerialDataReceivedEventHandler(port_DataReceived) ; 
           serial_error = new SerialErrorReceivedEventHandler(port_ErrorReceived);
           serial_disposed = new EventHandler(port_Disposed);
           UpdatePortList();
        }

        /// <summary>
        /// Updates list of port to reflect current state of the system's serial ports
        /// </summary>
        public void UpdatePortList()
        {
            serialPortNames = System.IO.Ports.SerialPort.GetPortNames();

            // check for validity of names in the list --> remove non-existant ports
            foreach (string name in connections.Keys)
            {
                var isValid = SerialPort.GetPortNames().Any(x => string.Compare(x, name, true) == 0);
                if (!isValid)
                {
                    Debug.Write(TAG + "UpdatePortList --> removing invalid name: " + name); 
                    connections.Remove(name);
                }
            }


            // add new names to the list --> add new ports (i.e. plugged-in arduino) 
            foreach (string name in serialPortNames)
            {
                // -------- new name detected
                if (!connections.Keys.Contains(name))
                {
                    SafeFileHandle hFile = CreateFile(@"\\.\" + name, -1073741824, 0, IntPtr.Zero, 3, dwFlagsAndAttributes, IntPtr.Zero);
                    if (!hFile.IsInvalid)
                    {
                        Debug.WriteLine(TAG + "UpdatePortList --> adding name: " + name); 
                        //throw new System.IO.IOException(string.Format("{0} port is already open", portName));
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
        public bool ConnectPort(string portName, ISerialPortDataListener listener,  SERIAL_SETTINGS settings)
        {
            // check if the port is known 
            if (portName != null && connections.Keys.Contains(portName) )
            {
                SerialPort port = connections[portName];
                if (port == null)
                {
                    port = new SerialPort(portName,
                                           settings.baudRate,
                                           settings.parity,
                                           settings.dataBits,
                                           settings.stopBits);
                    port.Open();

                }


                if (port.IsOpen)
                {
                   

                    port.DataReceived -= serial_received;
                    port.ErrorReceived -= serial_error;
                    port.Disposed -= serial_disposed;
                         
                    port.DataReceived += serial_received;
                    port.ErrorReceived += serial_error;
                    port.Disposed += serial_disposed;

                    connections[portName] = port;


                    OnDataReceived += new Action<string, byte[], int>(listener.port_DataReceived);

                    return true;
                }
                else
                {
                    Debug.WriteLine(TAG + "port did not open!");
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
        /// <param name="porName"></param>
        /// <param name="listener"></param>
        public void DisconnectListener(string portName, ISerialPortDataListener listener)
        {
           // Debug.WriteLine(TAG + "DisconnectListener");
           // if (portName != null && connections.Keys.Contains(portName) && connections[portName] == null)
           // {
           //SerialPort port = connections[portName];
            //    port.DataReceived -= listener.port_DataReceived; 
           // }
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
        public void WriteToPort(string portName,  byte[] buffer, int count)
        {
            Debug.WriteLine(TAG + "Port: " + portName + " sending " + count + " bytes ");
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

        /*
        /// <summary>
        /// Begins recursive registry enumeration
        /// </summary>
        /// <param name="oPortsToMap">array of port names (i.e. COM1, COM2, etc)</param>
        /// <returns>a hashtable mapping Friendly names to non-friendly port values</returns>
        Hashtable BuildPortNameHash(string[] oPortsToMap)
        {
            Hashtable oReturnTable = new Hashtable();
            try
            {
                MineRegistryForPortName("SYSTEM\\CurrentControlSet\\Enum", oReturnTable, oPortsToMap);
            }
            catch (Exception e)
            {
                Debug.WriteLine(TAG + e);
                return oReturnTable;
            }
            return oReturnTable;
        }
        /// <summary>
        /// Recursively enumerates registry subkeys starting with strStartKey looking for 
        /// "Device Parameters" subkey. If key is present, friendly port name is extracted.
        /// </summary>
        /// <param name="strStartKey">the start key from which to begin the enumeration</param>
        /// <param name="oTargetMap">hashtable that will get populated with 
        /// friendly-to-nonfriendly port names</param>
        /// <param name="oPortNamesToMatch">array of port names (i.e. COM1, COM2, etc)</param>
        void MineRegistryForPortName(string strStartKey, Hashtable oTargetMap, string[] oPortNamesToMatch)
        {
            if (oTargetMap.Count >= oPortNamesToMatch.Length)
                return;
            RegistryKey oCurrentKey = Registry.LocalMachine;
            oCurrentKey = oCurrentKey.OpenSubKey(strStartKey);
            string[] oSubKeyNames = oCurrentKey.GetSubKeyNames();
            if (oSubKeyNames.Contains("Device Parameters") && strStartKey != "SYSTEM\\CurrentControlSet\\Enum")
            {
                object oPortNameValue = Registry.GetValue("HKEY_LOCAL_MACHINE\\" +
                    strStartKey + "\\Device Parameters", "PortName", null);
                if (oPortNameValue == null ||
                    oPortNamesToMatch.Contains(oPortNameValue.ToString()) == false)
                    return;
                object oFriendlyName = Registry.GetValue("HKEY_LOCAL_MACHINE\\" +
                    strStartKey, "FriendlyName", null);
                string strFriendlyName = "N/A";
                if (oFriendlyName != null)
                    strFriendlyName = oFriendlyName.ToString();
                if (strFriendlyName.Contains(oPortNameValue.ToString()) == false)
                    strFriendlyName = string.Format("{0} ({1})", strFriendlyName, oPortNameValue);
                oTargetMap[strFriendlyName] = oPortNameValue;
            }
            else
            {
                foreach (string strSubKey in oSubKeyNames)
                    MineRegistryForPortName(strStartKey + "\\" + strSubKey, oTargetMap, oPortNamesToMatch);
            }
        }
        */


        /// <summary>
        /// Looks at the attached serial hardware and returns only those portnames that have an Insteon Module attached
        /// </summary>
        /// <returns>List of port name strings that correspond to an Arduino COM port</returns>
        public Dictionary<string, string> getInsteonPortNames()
        {
            Debug.WriteLine(TAG + "getInsteonPortNames"); 
            UpdatePortList();

            Dictionary<string,string> pNames = new Dictionary<string,string>();

            allowRX = false; 

            foreach (string portName in connections.Keys)
            {
                Debug.WriteLine(TAG + "Trying (A) " + portName);
              
                    if (connections[portName] == null)
                    {
                        try
                        {

                            INSTEON_USB_MODEM_SETTINGS settings = new INSTEON_USB_MODEM_SETTINGS();

                            SerialPort port = new SerialPort(portName,
                                                    settings.baudRate,
                                                    settings.parity,
                                                    settings.dataBits,
                                                    settings.stopBits);
                           
                            try
                            {
                                port.Open();
                              
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine(TAG + "Exception: " + e);
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
                            Debug.WriteLine(TAG + "ObjectDisposedException (A) " + e);
                            continue;
                        }
                       
                       continue;
                }
                else if (connections[portName].IsOpen)
                {
                    Debug.WriteLine(TAG + "Trying (B) " + portName);
                    try
                    {
                        SerialPort port = connections[portName];

                        if (port.BaudRate != 19200) continue;
                        //else: disable port data notifications for now
                        // ====================== disable receive events and sending on port during query! ===========
                        //port.DataReceived -= this.port_DataReceived;
                        allowWrite = false;
                       
                        // =================================================
                        Thread.Sleep(10);
                        // read out remaining bytes in port buffer
                        if (port.BytesToRead > 0)
                        {
                            byte[] dummy = new byte[1024];
                            port.Read(dummy, 0, port.BytesToRead);
                        }
                        // now queey
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
                        //port.DataReceived += this.port_DataReceived;
                        allowWrite = true;
                        // ============================================
                    }
                    catch ( ObjectDisposedException e)
                    {
                         Debug.WriteLine(TAG + "ObjectDisposedException (B) " + e);
                    }
                 }

               } // --------------- foreach


            allowRX = true; 

            return pNames;


        }


      

        /// <summary>
        /// Looks at the attached serial hardware and returns only those portnames that have an Arduiono attached
        /// </summary>
        /// <returns>List of port name strings that correspond to an Arduino COM port</returns>
        public Dictionary<string,string> getArduinoPortNames()
        {
            UpdatePortList();
            Dictionary<string,string> arduinoNames = new Dictionary<string,string>();

            /* ------ WMI Method MS_SerialPort doesn't work with my MS //REDMOND/ account :( */
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
                        Debug.WriteLine(TAG + "found Arduino UNO, Portname " + friendlyName);

                    }
                }
                
            }
            catch (ManagementException me)
            {
                Debug.WriteLine(TAG + "managementException: " + me);
                foreach (string key in connections.Keys)
                {
                    arduinoNames.Add(key, key); 
                }
            }
            
          

            //Hashtable m_oFriendlyNameMap = BuildPortNameHash(System.IO.Ports.SerialPort.GetPortNames());

            //arduinoNames;
            return arduinoNames;
            

        }


        #endregion



        //--------------------------------
        #region data received and other callbacks
        void port_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            Debug.WriteLine(TAG + "----> port_DataReceived <----");

            if (allowRX)
            {

                SerialPort p = sender as SerialPort;
                // new buffer 
                byte[] rx = new byte[32000];

                int nBytes = p.BytesToRead;
                if (nBytes > 0 && nBytes <= rx.Length)
                {
                    p.Read(rx, 0, nBytes);
                    if (VERBOSITY_LEVEL>0)
                    {
                        for (int i = 0; i < nBytes; i++)
                        {
                            Debug.Write(i + " "); Debug.WriteLine("{0,10:X}", rx[i]);
                        }
                    }
                }
                else if(nBytes > rx.Length )
                {
                    // overflow
                    p.DiscardInBuffer();
                }



                if (OnDataReceived != null)
                {
                    try
                    {
                        OnDataReceived(p.PortName, rx, nBytes);
                    }
                    catch (ObjectDisposedException ex)
                    {
                        Debug.WriteLine(TAG + "attempt to send data to disposed object " + ex);
                    }
                }
                //  hold on to a copy of the last buffer just in case
                rxBuf = rx;
            }
            else
            {
                Debug.WriteLine(TAG + "ignoring!");
            }
        }

        void port_ErrorReceived(object sender, SerialErrorReceivedEventArgs e)
        {
            //throw new NotImplementedException();
            Debug.WriteLine(TAG + "port_errorReceived " + e.ToString());
        }

        void port_Disposed(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            Debug.WriteLine(TAG + "port Disposed: " + e.ToString());
            SerialPort disposedPort = sender as SerialPort;

            connections.Remove(disposedPort.PortName);

           

        }
        #endregion











      
    }
}
