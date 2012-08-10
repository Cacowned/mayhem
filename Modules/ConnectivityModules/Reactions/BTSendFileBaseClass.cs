using System;
using System.Runtime.Serialization;
using Brecham.Obex;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using MayhemCore;

namespace ConnectivityModule.Reactions
{
    /// <summary>
    /// An abstract base class that is used for sending a file to a bluetooth device.
    /// </summary>
    [DataContract]
    public abstract class BTSendFileBaseClass : BTPairBaseClass
    {
        /// <summary>
        /// The path of the file that will be sent to the device.
        /// </summary>
        [DataMember]
        protected string filePath;

        protected BluetoothClient bluetoothClient;
        protected ObexClientSession session;

        protected void SendFileMethod()
        {
            try
            {
                bluetoothClient = new BluetoothClient();

                BluetoothEndPoint remoteEndPoint = new BluetoothEndPoint(device.DeviceAddress, BluetoothService.ObexObjectPush);

                bluetoothClient.Connect(remoteEndPoint);

                session = new ObexClientSession(bluetoothClient.GetStream(), UInt16.MaxValue);
                session.Connect();

                session.PutFile(filePath);

                session.Disconnect();
                session.Dispose();

                bluetoothClient.Dispose();
            }
            catch (PlatformNotSupportedException ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.BT_NoBluetooth);
                Logger.Write(ex);
            }
            catch (ObexResponseException ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.BT_ConnectionRefused);
                Logger.Write(ex);

                if (bluetoothClient != null)
                {
                    bluetoothClient.Dispose();
                }
            }
        }

        protected void Dispose()
        {
            if (session != null)
            {
                session.Disconnect();
                session.Dispose();
            }

            if (bluetoothClient != null)
            {
                bluetoothClient.Dispose();
            }
        }

        public abstract override void Perform();
    }
}
