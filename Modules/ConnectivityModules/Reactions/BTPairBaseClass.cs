using System.Runtime.Serialization;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using MayhemCore;

namespace ConnectivityModule.Reactions
{
    [DataContract]
    public abstract class BTPairBaseClass : ReactionBase
    {
        [DataMember]
        protected string accessPin;

        protected BluetoothDeviceInfo device;

        protected bool MakePairRequest()
        {
            BluetoothAddress deviceAddress = device.DeviceAddress;

            BluetoothSecurity.RemoveDevice(deviceAddress);

            if (!BluetoothSecurity.PairRequest(deviceAddress, accessPin))
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.BT_CantConnectToDevice);

                return false;
            }
            else
            {
                return true;
            }
        }

        public abstract override void Perform();
    }
}
