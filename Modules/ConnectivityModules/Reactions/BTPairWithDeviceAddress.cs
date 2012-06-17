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
    [MayhemModule("Bluetooth: Pair With Device By Address", "Pair with a specific device identified by its address")]
    public class BTPairWithDeviceAddress : ReactionBase, IWpfConfigurable
    {
        [DataMember]
        private string deviceAddressString;

        [DataMember]
        private string accessPin;

        public override void Perform()
        {
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

                BluetoothSecurity.RemoveDevice(deviceAddress);

                if (!BluetoothSecurity.PairRequest(deviceAddress, accessPin))
                    ErrorLog.AddError(ErrorType.Failure, Strings.BT_ErrorWrongPin);
                else
                    ErrorLog.AddError(ErrorType.Message, String.Format(Strings.BT_SuccessfulPairAddress, deviceAddress));
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
            get { return new PairWithDeviceByAddressConfig(deviceAddressString, accessPin); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            var config = configurationControl as PairWithDeviceByAddressConfig;

            if (config == null)
                return;

            deviceAddressString = config.DeviceAddress;
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
