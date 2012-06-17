using System;
using System.Globalization;
using System.Runtime.Serialization;
using Brecham.Obex;
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
    [MayhemModule("Bluetooth: Send File By Name", "Sends a file to a specific device identified by its name")]
    public class BTSendFileName : ReactionBase, IWpfConfigurable
    {
        [DataMember]
        private string deviceName;

        [DataMember]
        private string filePath;

        [DataMember]
        private string accessPin;        

        private BluetoothClient bluetoothClient;

        public override void Perform()
        {
            ObexClientSession session = null;

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
                    ErrorLog.AddError(ErrorType.Warning, string.Format(CultureInfo.CurrentCulture, Strings.BT_DeviceNotFound, deviceName));
                    return;
                }

                BluetoothAddress deviceAddress = device.DeviceAddress;

                BluetoothSecurity.PairRequest(deviceAddress, accessPin);

                BluetoothEndPoint remoteEndPoint = new BluetoothEndPoint(device.DeviceAddress, BluetoothService.ObexObjectPush);

                bluetoothClient.Connect(remoteEndPoint);

                session = new ObexClientSession(bluetoothClient.GetStream(), UInt16.MaxValue);
                session.Connect();

                session.PutFile(filePath);

                session.Disconnect();
                session.Dispose();

                bluetoothClient.Dispose();
            }
            catch (ObexResponseException ex)
            {
                ErrorLog.AddError(ErrorType.Warning, Strings.BT_ConnectionRefused);
                Logger.Write(ex);

                if (bluetoothClient != null)
                    bluetoothClient.Dispose();
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.BT_CantConnectToDevice);
                Logger.Write(ex);

                if (session != null)
                {
                    session.Disconnect();
                    session.Dispose();
                }

                if (bluetoothClient != null)
                    bluetoothClient.Dispose();
            }
        }

        #region IWpfConfigurable Methods

        public WpfConfiguration ConfigurationControl
        {
            get { return new SendFileToDeviceByNameConfig(deviceName, filePath, accessPin); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = configurationControl as SendFileToDeviceByNameConfig;

            if (config == null)
                return;

            deviceName = config.DeviceName;
            filePath = config.FilePath;
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
