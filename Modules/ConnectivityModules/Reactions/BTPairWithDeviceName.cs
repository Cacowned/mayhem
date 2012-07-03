using System;
using System.Globalization;
using System.Net.Sockets;
using System.Runtime.Serialization;
using ConnectivityModule.Wpf;
using InTheHand.Net.Sockets;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;

namespace ConnectivityModule.Reactions
{
    /// <summary>
    /// A class that pairs the computer with a bluetooth device identified by it's name.
    /// </summary>
    [DataContract]
    [MayhemModule("Bluetooth: Pair With Device By Name", "Pair with a specific device identified by its name")]
    public class BTPairWithDeviceName : BTPairBaseClass, IWpfConfigurable
    {
        /// <summary>
        /// The name of the device.
        /// </summary>
        [DataMember]
        private string deviceName;

        private BluetoothClient bluetoothClient;

        /// <summary>
        /// This method will try to pair the computer with the bluetooth device indentified by it's name.
        /// </summary>
        public override void Perform()
        {
            try
            {
                device = null;

                bluetoothClient = new BluetoothClient();

                BluetoothDeviceInfo[] devices = bluetoothClient.DiscoverDevices(10, false, false, false, true);

                for (int i = 0; i < devices.Length; i++)
                {
                    if (devices[i].DeviceName.Equals(deviceName))
                    {
                        device = devices[i];
                        break;
                    }
                }

                // Adding a warning message if the device is not in range.
                if (device == null)
                {
                    ErrorLog.AddError(ErrorType.Failure, string.Format(CultureInfo.CurrentCulture, Strings.BT_DeviceNotFound, deviceName));
                    return;
                }

                if (MakePairRequest())
                {
                    ErrorLog.AddError(ErrorType.Message, string.Format(CultureInfo.CurrentCulture, Strings.BT_SuccessfulPair, deviceName));
                }
            }
            catch (SocketException ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.BT_CantConnectToDevice);
                Logger.Write(ex);
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.BT_CantPairWithDevice);
                Logger.Write(ex);
            }
        }

        #region IWpfConfigurable Methods

        public WpfConfiguration ConfigurationControl
        {
            get { return new PairWithDeviceByNameConfig(deviceName, accessPin); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = configurationControl as PairWithDeviceByNameConfig;

            if (config == null)
            {
                return;
            }

            deviceName = config.DeviceName;
            accessPin = config.Pin;
        }

        #endregion

        #region IWpfConfigurable Members

        public string GetConfigString()
        {
            return string.Format(CultureInfo.CurrentCulture, Strings.DevicePairName_ConfigString, deviceName, accessPin);
        }

        #endregion
    }
}
