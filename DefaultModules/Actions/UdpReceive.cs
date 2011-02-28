using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using DefaultModules.UdpHelpers;
using MayhemCore;
using MayhemCore.ModuleTypes;

namespace DefaultModules.Actions
{
    [Serializable]
    public class UdpReceive : ActionBase, ICli, ISerializable
    {
        public const string TAG = "[UdpReceive]";

        public static HashSet<int> usedPorts = new HashSet<int>();

        protected int port = 1111;

        protected AsyncUdpServerSocket socket = null;

        protected string listenMessage = null;

        protected AsyncUdpServerSocket.DataReceivedHandler receivedHandler = null;

        public UdpReceive()
            : base("UDP Receive", "Fires when predefined UDP message is received")
        {
            hasConfig = true;
            SetListenOnPort(port, listenMessage);
        }

        public void CliConfig()
        {
            int portNum = 0;
            string text = "";


            string input = "";
            do 
            {
                Console.Write("{0} Enter port to listen on (>1023): ", TAG);
                input = Console.ReadLine();
            } while(!Int32.TryParse(input, out portNum) || !(portNum > 1023));

            do
            {
                Console.Write("{0} Enter text to listen for: ", TAG);
                text = Console.ReadLine();
            } while (text.Trim().Length == 0);

            SetListenOnPort(port, text);
        }

        public void SetListenOnPort(int port, string message) {
            this.port = port;
            this.listenMessage = message;

            socket = AsyncUdpServerSocket.GetSocketForPort(port);
            receivedHandler = new AsyncUdpServerSocket.DataReceivedHandler(socket_OnDataReceived);

            ConfigString = String.Format("Port: {0}, Message: \"{1}\"", port, listenMessage);
        }


        public override void Enable() {
            base.Enable();
            if (socket != null)
                socket.OnDataReceived += receivedHandler;
        }

        void socket_OnDataReceived(object sender, DataReceivedEventArgs e) {

            if (e.data == listenMessage) {
                base.OnActionActivated();
            }
        }

        public override void Disable() {
            base.Disable();
            if (socket != null)
                socket.OnDataReceived -= receivedHandler;
        }

        #region Serialization

        public UdpReceive(SerializationInfo info, StreamingContext context) : base (info, context)
        {
            //throw new NotImplementedException();

            this.port = info.GetInt32("Port");
            this.listenMessage = info.GetString("ListenMessage");

            SetListenOnPort(port, listenMessage);

        }

        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //throw new NotImplementedException();
            base.GetObjectData(info, context);
            info.AddValue("Port", port);
            info.AddValue("ListenMessage", listenMessage);

        }
        #endregion

        ~UdpReceive()
        {
            socket = null;
        }
    }
}
