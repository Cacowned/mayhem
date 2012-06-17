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
    [MayhemModule("Bluetooth: Send File By Address", "Sends a file to a specific device identified by its address")]
    public class BTSendFileAddress : ReactionBase, IWpfConfigurable
    {
        [DataMember]
        private string deviceAddressString;

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
                BluetoothDeviceInfo device = new BluetoothDeviceInfo(BluetoothAddress.Parse(deviceAddressString));

                // Adding a warning message if the device is not in range
                if (device == null)
                {
                    ErrorLog.AddError(ErrorType.Warning, string.Format(CultureInfo.CurrentCulture, Strings.BT_DeviceAddressNotFound, deviceAddressString));
                    return;
                }

                BluetoothAddress deviceAddress = device.DeviceAddress;

                BluetoothSecurity.PairRequest(deviceAddress, accessPin);

                BluetoothEndPoint remoteEndPoint = new BluetoothEndPoint(device.DeviceAddress, BluetoothService.ObexObjectPush);

                bluetoothClient = new BluetoothClient();
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
            get { return new SendFileToDeviceByAddressConfig(deviceAddressString, filePath, accessPin); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = configurationControl as SendFileToDeviceByAddressConfig;

            if (config == null)
                return;

            deviceAddressString = config.DeviceAddress;
            filePath = config.FilePath;
            accessPin = config.Pin;
        }

        #endregion

        #region IWpfConfigurable Members

        public string GetConfigString()
        {
            return string.Format(CultureInfo.CurrentCulture, Strings.DeviceAddress_ConfigString, deviceAddressString);
        }

        #endregion
    }
}
