using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using MayhemCore.ModuleTypes;
using DefaultModules.UdpHelpers;

namespace DefaultModules.Actions
{
    
    public class UdpReceive : ActionBase, ICli
    {
        public const string TAG = "[UdpReceive]";

        public static HashSet<int> used_ports = new HashSet<int>();

        protected int port = 1111;

        protected AsyncUdpServerSocket socket = null;

        protected string listen_message = null;

        protected AsyncUdpServerSocket.DataReceivedHandler received_handler = null;

        public UdpReceive()
            : base("UDP Receive", "Fires when predefined UDP message is received")
        {
            hasConfig = true;
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
            this.listen_message = message;

            socket = AsyncUdpServerSocket.GetSocketForPort(port);
            received_handler = new AsyncUdpServerSocket.DataReceivedHandler(socket_OnDataReceived);
        }


        public override void Enable() {
            base.Enable();
            if (socket != null)
                socket.OnDataReceived += received_handler;
        }

        void socket_OnDataReceived(object sender, DataReceivedEventArgs e) {

            if (e.data == listen_message) {
                base.OnActionActivated();
            }
        }

        public override void Disable() {
            base.Disable();
            if (socket != null)
                socket.OnDataReceived -= received_handler;
        }
    }
}
