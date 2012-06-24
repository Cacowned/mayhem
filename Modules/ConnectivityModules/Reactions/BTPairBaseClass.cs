using System.Runtime.Serialization;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using MayhemCore;

namespace ConnectivityModule.Reactions
{
    /// <summary>
    ///  An abstract base class used for pairing the computer with the bluetooth device.
    /// </summary>
    [DataContract]
    public abstract class BTPairBaseClass : ReactionBase
    {
        /// <summary>
        /// The access pin used for connection.
        /// </summary>
        [DataMember]
        protected string accessPin;

        protected BluetoothDeviceInfo device;

        /// <summary>
        /// This method will try to pair the computer with a bluetooth device.
        /// </summary>
        /// <returns>Returns true if the pair was successful, </returns>
        protected bool MakePairRequest()
        {
            BluetoothAddress deviceAddress = device.DeviceAddress;

            // If the device was previously paired with the computer it will be removed so a new pair could be done.
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

        /// <summary>
        /// This method will be implemented by the classes that inherit this class and will be called when the event associated with the reaction is triggered.
        /// It contains the functionality of this reaction.
        /// </summary>
        public abstract override void Perform();
    }
}
