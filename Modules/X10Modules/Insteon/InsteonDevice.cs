/*
 * InsteonDevice.cs
 * 
 * Stores data about Insteon Devices linked to Modem
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

namespace X10Modules.Insteon
{
    /// <summary>
    /// Store data about Insteon Devices linked to Modem
    /// </summary>
    public class InsteonDevice
    {
        public static Dictionary<int, InsteonDevice>  devices = new Dictionary<int,InsteonDevice>(); 

        public string name = string.Empty;

        private byte[] deviceID_ = new byte[3];
        public byte[] deviceID
        {
            get { return deviceID_; }
            set
            {
                // assert that the value length is always 3 bytes
                if (value.Length != 3)
                {
                    throw new NotSupportedException();
                }
                else
                {
                    deviceID_ = value;
                }
            }        
        }

        public bool powerState = false; 
        public byte ALRecordFlags = 0;
        public byte ALGroup = 0;

        private byte[] linkData_ = new byte[3];
        public byte[] linkData
        {
            get { return linkData_; }
            set
            {
                // assert that the value length is always 3 bytes
                if (value.Length != 3)
                {
                    throw new NotSupportedException();
                }
                else
                {
                    linkData_ = value;
                }
            }
        }

        /// <summary>
        /// Gets power state for a given device address
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        public static bool GetPowerStateForDeviceAddress(byte[] address)
        {
            if (address.Length != 3)
            {
                throw new NotSupportedException();          
            }
            
            int dev_hash = DeviceHash(address);

            if (devices.Keys.Count > 0 && devices.Keys.Contains(dev_hash))
            {
                InsteonDevice d = devices[dev_hash];
                return d.powerState; 
            }
            else
            {             
                return false;
            }
        }

        /// <summary>
        /// build a "hash" of an insteon device address by summing the device address values in a clever way
        /// </summary>
        /// <param name="address"></param>
        /// <returns></returns>
        private static int DeviceHash(byte[] address)
        {
            if (address.Length == 3)
            {
                return address[0] + (address[1] << 8) + (address[2] << 16);
            }
            else
            {
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Sets the power state for a given device address. 
        /// </summary>
        /// <param name="address"></param>
        /// <param name="state"></param>
        public static void SetPowerStateForDeviceAddress(byte[] address, bool state)
        {
            if (address.Length != 3)
            {
                throw new NotSupportedException(); 
            }

            int dev_hash = DeviceHash(address);

            if (devices.Keys.Count > 0 && devices.Keys.Contains(dev_hash))
            {
                InsteonDevice d = devices[dev_hash];
                d.powerState = state; 
            }
            else
            {
                devices[dev_hash] = new InsteonDevice();
                devices[dev_hash].powerState = state;
            }
        }

        /// <summary>
        /// Returns a friendly name for display in combo box lists (e.g.) 
        /// </summary>
        /// <returns>string with the name</returns>
        public string ListName
        {
            get { return String.Format("{0}-{1:x2}:{2:x2}:{3:x2}", ALGroup, deviceID[0], deviceID[1], deviceID[2]); }
        }

        public override string ToString()
        {            
            string str = String.Format("deviceID {0:x2}:{1:x2}:{2:x2} linkdata {3:x4}:{4:x4}:{5:x4} ALrFlags {6:x4} ALGroup {7:x4}",
                                           deviceID[0], deviceID[1], deviceID[2],
                                           linkData[0], linkData[1], linkData[2],
                                           ALRecordFlags,
                                           ALGroup);
            return str;
        }
    }
}
