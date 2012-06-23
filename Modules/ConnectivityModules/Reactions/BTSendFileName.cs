using System;
using System.Globalization;
using System.Runtime.Serialization;
using ConnectivityModule.Wpf;
using InTheHand.Net.Sockets;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;

namespace ConnectivityModule.Reactions
{
    [DataContract]
    [MayhemModule("Bluetooth: Send File By Name", "Sends a file to a specific device identified by its name")]
    public class BTSendFileName : BTSendFileBaseClass, IWpfConfigurable
    {
        [DataMember]
        private string deviceName;

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

                // Adding a warning message if the device is not in range
                if (device == null)
                {
                    ErrorLog.AddError(ErrorType.Failure, string.Format(CultureInfo.CurrentCulture, Strings.BT_DeviceNotFound, deviceName));
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
            return string.Format(CultureInfo.CurrentCulture, Strings.DevicePairName_ConfigString, deviceName, accessPin);
        }

        #endregion
    }
}
