using System;
using System.Globalization;
using System.Net.Sockets;
using System.Runtime.Serialization;
using ConnectivityModule.Wpf;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;

namespace ConnectivityModule.Reactions
{
    [DataContract]
    [MayhemModule("Bluetooth: Pair With Device By Name", "Pair with a specific device identified by its name")]
    public class BTPairWithDeviceName : ReactionBase, IWpfConfigurable
    {
        [DataMember]
        private string deviceName;

        [DataMember]
        private string accessPin;

        private BluetoothClient bluetoothClient;

        public override void Perform()
        {
            try
            {
                BluetoothDeviceInfo device = null;

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

                // Adding a warning message if the device is not in range
                if (device == null)
                {
                    ErrorLog.AddError(ErrorType.Failure, string.Format(CultureInfo.CurrentCulture, Strings.BT_DeviceNotFound, deviceName));
                    return;
                }

                BluetoothAddress deviceAddress = device.DeviceAddress;

                BluetoothSecurity.RemoveDevice(deviceAddress);

                if (!BluetoothSecurity.PairRequest(deviceAddress, accessPin))
                    ErrorLog.AddError(ErrorType.Failure, Strings.BT_ErrorWrongPin);
                else
                    ErrorLog.AddError(ErrorType.Message, String.Format(Strings.BT_SuccessfulPairAddress, deviceName));
            }
            catch (SocketException ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.BT_ErrorWrongPin);
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
                return;

            deviceName = config.DeviceName;
            accessPin = config.Pin;
        }

        #endregion

        #region IWpfConfigurable Members

        public string GetConfigString()
        {
            return string.Format(CultureInfo.CurrentCulture, Strings.DeviceName_ConfigString, deviceName);
        }

        #endregion
    }
}
