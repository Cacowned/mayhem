using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Net.Sockets;
using System.Net;
using MayhemApp.Low_Level;
using MayhemApp.Dialogs;
using System.Diagnostics;

namespace MayhemApp.Business_Logic.Triggers
{
    [Serializable]
    class MayhemUDPTrigger : MayhemTriggerBase, ISerializable, IMayhemTriggerCommon, IMayhemConnectionItemCommon
    {

        public static HashSet<int> used_ports = new HashSet<int>();

        public static string TAG = "[MayhemUDPTrigger] :";

        private int port = 1111;


        private AsyncUDPServerSocket socket = null;

        private string listen_message = null;

        public override event triggerActivateHandler onTriggerActivated;

        private AsyncUDPServerSocket.DataReceivedHandler received_handler = null;


        #region constructors

        public MayhemUDPTrigger(string s)
            : this() { }

        public MayhemUDPTrigger()
            : base("UDP Receive",
                   "Fires when predefined UDP message is received",
                   "This trigger fires when it receives a predefined UDP message. Double click to define the message and the receiving port.")
        {
     
            setup_window = new UDPTriggerSetupWindow();

            ((UDPTriggerSetupWindow)setup_window).OnSetButtonClicked += new UDPTriggerSetupWindow.SetButtonClickedHandler_(MayhemUDPTrigger_OnSetButtonClicked);

            this.template_data.SubTitle = "Listening for: " + ((UDPTriggerSetupWindow)setup_window).tweet_text + "\n" + "Port: " + ((UDPTriggerSetupWindow)setup_window).portBox.Text;
        }

   

        void MayhemUDPTrigger_OnSetButtonClicked(object sender, TwitterActionSetupWindow.TwitterActionSetTextEventArgs e)
        {

            UDPTriggerSetupWindow.UDPTriggerEventArgs args = e as UDPTriggerSetupWindow.UDPTriggerEventArgs;

            // TODO avoid this hack
            /*
            MayhemButtonControl associated_control = this.template_data.control as MayhemButtonControl;
            if (associated_control != null)
             ((MayhemButtonControl)this.template_data.control).sTitle.Text = "Listening for: " + ((UDPTriggerSetupWindow)setup_window).tweet_text + "\n" + "Port: " + ((UDPTriggerSetupWindow)setup_window).portBox.Text;
            */
            this.template_data.SubTitle = "Listening for: " + ((UDPTriggerSetupWindow)setup_window).tweet_text + "\n" + "Port: " + ((UDPTriggerSetupWindow)setup_window).portBox.Text;

            listen_message = args.message;
            port = args.port;

            SetListenOnPortWithMessage(port, listen_message);


        }
        #endregion

        

        public void SetListenOnPortWithMessage(int portNr, string message)
        {
             port = portNr;
             listen_message = message;
             socket = AsyncUDPServerSocket.GetSocketForPort(port);
             received_handler = new AsyncUDPServerSocket.DataReceivedHandler(socket_OnDataReceived);
        }

        # region MayhemTrigger overrides

        public override void EnableTrigger()
        {
            base.EnableTrigger();
            if (socket != null)
                socket.OnDataReceived += received_handler;
        }

        void socket_OnDataReceived(object sender, Low_Level.DataReceivedEventArgs e)
        {

            if (e.data == listen_message)
            {
                if (onTriggerActivated != null)
                {
                    onTriggerActivated(this, new EventArgs());
                }
            }
        }

        public override void DisableTrigger()
        {
            if (socket != null)
                socket.OnDataReceived -= received_handler;
        }

        public override void OnDoubleClick(object sender, System.Windows.Input.MouseEventArgs e)
        {
            DimMainWindow(true);
            setup_window.ShowDialog();
            DimMainWindow(false);
        }

        #endregion

        #region Serialization

        public MayhemUDPTrigger(SerializationInfo info, StreamingContext context) : base (info, context)
        {
            //throw new NotImplementedException();

            this.port = info.GetInt32("Port");
            this.listen_message = info.GetString("ListenMessage");

            setup_window = new UDPTriggerSetupWindow();

            ((UDPTriggerSetupWindow)setup_window).tweet_text = this.listen_message;
            ((UDPTriggerSetupWindow)setup_window).portBox.Text = this.port.ToString();
            ((UDPTriggerSetupWindow)setup_window).OnSetButtonClicked += new UDPTriggerSetupWindow.SetButtonClickedHandler_(MayhemUDPTrigger_OnSetButtonClicked);

            this.template_data.SubTitle = "Listening for: " + ((UDPTriggerSetupWindow)setup_window).tweet_text + "\n" + "Port: " + ((UDPTriggerSetupWindow)setup_window).portBox.Text;

            SetListenOnPortWithMessage(port, listen_message);

        }

        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //throw new NotImplementedException();

            info.AddValue("Port", port);
            info.AddValue("ListenMessage", listen_message);

        }
        #endregion

        ~MayhemUDPTrigger()
        {
            socket = null;
        }
    }
}
