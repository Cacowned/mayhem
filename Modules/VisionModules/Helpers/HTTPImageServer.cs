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
        private AutoResetEvent listenerThreadLock = new AutoResetEvent(false);
        private Bitmap showBitmap; 

        public HTTPImageServer(int port )
        {
            
            this.port = port;
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

        public void ProcessRequestsThread(object o)
        {
            while (threadRunning)
            {
                IAsyncResult result = httpListener.BeginGetContext(new AsyncCallback(ListenerCallback), httpListener);
                Logger.WriteLine("Async request handled");
                listenerThreadLock.WaitOne();
            }
            threadStopEvent.Set();
            Logger.WriteLine("ProcessRequestThread Stopped!");
           
        }

        public void ListenerCallback(IAsyncResult result)
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
                Random r = new Random(DateTime.Now.Millisecond);
                int px = (int)( r.NextDouble() * (double) showBitmap.Width);
                int py = (int)( r.NextDouble() * (double)showBitmap.Height);

                Graphics g = Graphics.FromImage(showBitmap);

              //  g.DrawArc(new SolidBrush(), px, py, 10, 10, 0, 360); 
                


                showBitmap.SetPixel(px, py, Color.Red);
                Logger.WriteLine("Sending Image File");
                response.ContentType = "image/jpeg";
                System.IO.Stream output = response.OutputStream;
                showBitmap.Save(output, ImageFormat.Jpeg);
                output.Close();
            }
            else
            {
                string responseString = "<HTML><HEAD><META HTTP-EQUIV=\"REFRESH\" CONTENT=\"1\"></HEAD><BODY><IMG SRC=\"mayhem.jpg\"></IMG></BODY></HTML>";
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
