using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Net;
using System.Threading;
using MayhemCore;
using System.IO;
using System.Drawing.Imaging;
using MayhemOpenCVWrapper;

namespace VisionModules.Helpers
{
  
    class HTTPImageServer
    {
        private string prefix;
        private static int port = 8080;
        private HttpListener httpListener = new HttpListener();
        private bool threadRunning = true;
        private AutoResetEvent threadStopEvent = new AutoResetEvent(false);
        private AutoResetEvent listenerThreadLock = new AutoResetEvent(false);
        private AutoResetEvent refresh = new AutoResetEvent(false);
        private Bitmap showBitmap;
        private static Dictionary<ImagerBase, HTTPImageServer> servers = new Dictionary<ImagerBase, HTTPImageServer>();

        public int Port
        {
            get;
            private set;
        }

        public static HTTPImageServer ServerForCamera(ImagerBase camera)
        {
            if (servers.Keys.Contains(camera))
            {
                return servers[camera];
            }
            else
            {
                HTTPImageServer s = new HTTPImageServer(port);
                port++;
                servers[camera] = s;
                return servers[camera];
            }
        }

        private HTTPImageServer(int aPort )
        {
            
            this.Port = aPort;
            prefix = "http://localhost:" + port + "/";
            httpListener.Prefixes.Add(prefix);
            showBitmap = new Bitmap(640, 480, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
        }

        public void StartServer()
        {
            Logger.WriteLine("Starting Server");
            httpListener.Start();
            threadRunning = true; 
            ThreadPool.QueueUserWorkItem((o) => ProcessRequestsThread(o));

        }

        public void StopServer()
        {
            Logger.WriteLine("Stopping Server");
            threadRunning = false;
            listenerThreadLock.Set();
            threadStopEvent.WaitOne();
            
          
        }

        /// <summary>
        /// Update Image to be served
        /// </summary>
        /// <param name="b">Input Image</param>
        public void UpdateImage(Bitmap b)
        {
            lock (showBitmap)
            {
                if (showBitmap != null)
                    showBitmap.Dispose();
                showBitmap = b;
            }
            // allow refresh
            refresh.Set();
        }

        private void ProcessRequestsThread(object o)
        {
            while (threadRunning)
            {
                IAsyncResult result = httpListener.BeginGetContext(new AsyncCallback(ListenerCallback), httpListener);
                Logger.WriteLine("Async request handled");
                // wait for last listenercallback to terminate
                listenerThreadLock.WaitOne();
               
            }
            threadStopEvent.Set();
            Logger.WriteLine("ProcessRequestThread Stopped!");
           
        }

        private void ListenerCallback(IAsyncResult result)
        {
           
            HttpListener listener = result.AsyncState as HttpListener;
            HttpListenerContext context = listener.EndGetContext(result);
           
            Logger.WriteLine("Got Request Url: " + context.Request.Url);
            // Obtain a response object.
            HttpListenerResponse response = context.Response;
            // Construct a response.
            string[] segments = context.Request.Url.Segments;

            if (segments.Last().Equals("mayhem.jpg"))
            {           
                Logger.WriteLine("Sending Image File");
                response.ContentType = "image/jpeg";
                System.IO.Stream output = response.OutputStream;
                showBitmap.Save(output, System.Drawing.Imaging.ImageFormat.Jpeg);              
                output.Close();
            }
            else if (segments.Last().Equals("/"))
            {
                
                string responseString = "<HTML><HEAD><META HTTP-EQUIV=\"REFRESH\" CONTENT=\"1\"></HEAD><BODY><IMG SRC=\"mayhem.jpg\"></IMG></BODY></HTML>";
                // wait for refresh lock to be released
                refresh.WaitOne();
                byte[] buffer = System.Text.Encoding.UTF8.GetBytes(responseString);
                // Get a response stream and write the response to it.
                response.ContentLength64 = buffer.Length;
                System.IO.Stream output = response.OutputStream;
                output.Write(buffer, 0, buffer.Length);
                // You must close the output stream.
                output.Close();             
            }
            listenerThreadLock.Set();
        }
    }
}
