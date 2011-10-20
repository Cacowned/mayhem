using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Net;
using System.Threading;
using MayhemCore;

namespace VisionModules.Helpers
{
  
    class HTTPImageServer
    {
        private string prefix;
        private Bitmap serveImage; 
        private int port;
        private HttpListener httpListener = new HttpListener();
        private bool threadRunning = true;
        private AutoResetEvent threadStopEvent = new AutoResetEvent(false);

        public HTTPImageServer(int port )
        {
            
            this.port = port;
            prefix = "http://localhost:" + port + "/";
            httpListener.Prefixes.Add(prefix);         
        }

        public void StartServer()
        {
            Logger.WriteLine("Starting Server");
            httpListener.Start();
            ThreadPool.QueueUserWorkItem((o) => ProcessRequestsThread(o));

        }

        public void StopServer()
        {
            Logger.WriteLine("Stopping Server");
            threadRunning = false;
            threadStopEvent.WaitOne();
            httpListener.Stop();
        }

        public void ProcessRequestsThread(object o)
        {
            if (threadRunning)
            {
                IAsyncResult result = httpListener.BeginGetContext(new AsyncCallback(ListenerCallback), httpListener);
                result.AsyncWaitHandle.WaitOne();
                Logger.WriteLine("Async request handled");
            }
            threadStopEvent.Set();
        }

        public void ListenerCallback(IAsyncResult result)
        {
           
            HttpListener listener = result.AsyncState as HttpListener;
            HttpListenerContext context = listener.EndGetContext(result);
            Logger.WriteLine("Got Request Url: " + context.Request.Url);
            // Obtain a response object.
            HttpListenerResponse response = context.Response;
            // Construct a response.
            string responseString = "<HTML><BODY><IMG src=\"mayhem.jpg\"><IMG/></BODY></HTML>";
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
            // Get a response stream and write the response to it.
            response.ContentLength64 = buffer.Length;
            System.IO.Stream output = response.OutputStream;
            output.Write(buffer, 0, buffer.Length);
            // You must close the output stream.
            output.Close();
        }
    }
}
