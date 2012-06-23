using System;
using System.Globalization;
using System.Runtime.Serialization;
using ConnectivityModule.Wpf;
using InTheHand.Net;
using InTheHand.Net.Sockets;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;

namespace ConnectivityModule.Reactions
{
    [DataContract]
    [MayhemModule("Bluetooth: Send File By Address", "Sends a file to a specific device identified by its address")]
    public class BTSendFileAddress : BTSendFileBaseClass, IWpfConfigurable
    {
        [DataMember]
        private string deviceAddressString;

        public override void Perform()
        {
            try
            {
                device = null;

                bluetoothClient = new BluetoothClient();

                device = new BluetoothDeviceInfo(BluetoothAddress.Parse(deviceAddressString));

                // Adding a warning message if the device is not in range
                if (device == null)
                {
                    ErrorLog.AddError(ErrorType.Failure, string.Format(CultureInfo.CurrentCulture, Strings.BT_DeviceAddressNotFound, deviceAddressString));
                    return;
                }

                if (!MakePairRequest())
                {
                    // If we could not pair we can't send the file
                    return;
                }

                SendFileMethod();
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.BT_CantConnectToDevice);
                Logger.Write(ex);

                Dispose();
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
