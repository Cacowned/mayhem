using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace DefaultModules.UdpHelpers
{
    public class DataReceivedEventArgs : EventArgs
    {
        public string data = null;
    }

    class AsyncUdpServerSocket
    {
        // port nr -- socket
        private static Dictionary<int, AsyncUdpServerSocket> sockets_ = new Dictionary<int, AsyncUdpServerSocket>();

        public static string TAG = "[AsyncUDPSocket] :";

        private IPEndPoint endPoint;
        private UdpClient client;

        private bool receive = false;
        private int myPort; 

        public delegate void DataReceivedHandler(object sender, DataReceivedEventArgs e);
        public event DataReceivedHandler OnDataReceived; 


        /**<summary>
         * Factory method for Async Sockets in Mayhem
         * </summary>
         */ 
        public static AsyncUdpServerSocket GetSocketForPort(int port)
        {
            
            if (sockets_.ContainsKey(port))
            {
                return sockets_[port];
            }
            else
            {
                return new AsyncUdpServerSocket(port);
            }

        }

        /**<summary>
         * Actual socket constructor: should not be called externally
         * </summary>
         */ 
        private AsyncUdpServerSocket(int port)
        {
            myPort = port;
            endPoint = new IPEndPoint(IPAddress.Any, port);
            client = new UdpClient(port);
            // register this socket on the list of used ports
            sockets_.Add(port,this);

            receive = true;
            // start receiving directly
            client.BeginReceive(new AsyncCallback(DataReceive), new UdpState() { e = endPoint, u = client });
        }

        /**<summary>
        * Asynchronous callback when data is received on the port.
        * </summary>
        */
        public void DataReceive(IAsyncResult ar)
        {

            Debug.WriteLine(TAG, "DataReceive");

            UdpClient u = (UdpClient)((UdpState)(ar.AsyncState)).u;
            IPEndPoint e = (IPEndPoint)((UdpState)(ar.AsyncState)).e;

            byte[] receivedBytes = u.EndReceive(ar, ref e);
            string message = Encoding.ASCII.GetString(receivedBytes);


            // generate an event for listeners on this port
            if (OnDataReceived != null)
            {
                OnDataReceived(this, new DataReceivedEventArgs() { data = message });
            }

            // continue async listening
            if (receive) 
                u.BeginReceive(new AsyncCallback(DataReceive), new UdpState() { e = endPoint, u = u });
        }

        public void StartReceive()
        {
            receive = true;
        }

        public void StopReceive()
        {
            receive = false;
        }

        public void ShutDown()
        {
            StopReceive();
            client.Close();
            sockets_.Remove(myPort);
        }

        ~AsyncUdpServerSocket()
        {
            if (sockets_.ContainsValue(this))
            {
                if (receive)
                {
                    this.ShutDown();
                }
                else
                {
                    sockets_.Remove(this.myPort);
                }
            }
        }
    }
}
