using System;
using System.Collections.Generic;
using System.Linq;

namespace X10Modules.Insteon
{
    /// <summary>
    /// Store data about Insteon Devices linked to Modem
    /// </summary>
    public class InsteonDevice
    {
        private static readonly Dictionary<int, InsteonDevice> Devices = new Dictionary<int, InsteonDevice>();

        private byte[] deviceId = new byte[3];

        public byte[] DeviceId
        {
            get
            {
                return deviceId;
            }
            set
            {
                // assert that the value length is always 3 bytes
                if (value.Length != 3)
                {
                    throw new NotSupportedException();
                }

                deviceId = value;
            }
        }

        public byte AlRecordFlags
        {
            get;
            set;
        }

        public byte ALGroup
        {
            get;
            set;
        }

        private bool powerState;

        private byte[] linkData = new byte[3];

        public byte[] LinkData
        {
            get
            {
                return linkData;
            }
            set
            {
                // assert that the value length is always 3 bytes
                if (value.Length != 3)
                {
                    throw new NotSupportedException();
                }

                linkData = value;
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

            int devHash = DeviceHash(address);

            if (Devices.Keys.Count > 0 && Devices.Keys.Contains(devHash))
            {
                InsteonDevice d = Devices[devHash];
                return d.powerState;
            }

            return false;
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

            throw new NotSupportedException();
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

            int devHash = DeviceHash(address);

            if (Devices.Keys.Count > 0 && Devices.Keys.Contains(devHash))
            {
                InsteonDevice d = Devices[devHash];
                d.powerState = state;
            }
            else
            {
                Devices[devHash] = new InsteonDevice();
                Devices[devHash].powerState = state;
            }
        }

        /// <summary>
        /// Returns a friendly name for display in combo box lists (e.g.) 
        /// </summary>
        /// <returns>string with the name</returns>
        public string ListName
        {
            get { return string.Format("{0}-{1:x2}:{2:x2}:{3:x2}", ALGroup, DeviceId[0], DeviceId[1], DeviceId[2]); }
        }

        public override string ToString()
        {
            string str = string.Format("deviceID {0:x2}:{1:x2}:{2:x2} linkdata {3:x4}:{4:x4}:{5:x4} ALrFlags {6:x4} ALGroup {7:x4}",
                                           DeviceId[0],
                                           DeviceId[1],
                                           DeviceId[2],
                                           LinkData[0],
                                           LinkData[1],
                                           LinkData[2],
                                           AlRecordFlags,
                                           ALGroup);
            return str;
        }
    }
}
