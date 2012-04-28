using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows;
using System.Threading;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.IO;
using System.Drawing.Imaging;
using System.Windows.Threading;


namespace MayhemWebCamWrapper
{
    //this class extends the image control so that it can be directly used for rendering!
    public class WebCam : ImagerBase, IDisposable, INotifyPropertyChanged 
    {
        
        //constructors...
        public WebCam(int captureWidth=640, int captureHeight=480)
        {
            Subscribers = new List<ImageListenerBase>();
            _index = -1;
            _name = "No camera detected";
            _path = default(String);
            _isavailable = false;
            _width = captureWidth;
            _height = captureHeight;
        }

        public WebCam(string name, string path, int index, bool isavailable)
        {
            Subscribers = new List<ImageListenerBase>();
            _name = name;
            _path = path;
            _index = index;
            _isavailable = isavailable;
            _width = 640;
            _height = 480;
        }

        
        public override void Start()
        {
            
            if (IsAvailable())
            {
                Stop();
                if (_height == default(int) || _width == default(int))
                {
                    _height = 480;
                    _width = 640;
                }

                if (worker == null)
                {
                    BitmapReady += OnBitmapReady;
                    SetBuffer();
                
                    stopSignal = new ManualResetEvent(false);
                    worker = new Thread(RunWorker);
                    worker.Start();
                }
              
            }
          //  else
          //  {
           //     string str = "Another process is using " + _name;
            //    throw new ImageException(str);
           // }
        }
        public override void Stop()
        {
            if (IsActive)
            {
                stopSignal.Set();
                worker.Abort();
                if (worker != null)
                {
                    worker.Join();
                    Release();
                }
            }

        }

        public bool IsAvailable() { return _isavailable; }
        public int WebCamID
        {
            get { return _index; }
        }
        public string WebCamName
        {
            get { return _name; }
        }

        public string WebCamPath
        {
            get { return _path; }
        }

        public override int Width
        {
            get { return _width; }
            set { _width = value; }
        }
        public override int Height
        {
            get { return _height; }
            set { _height = value; }
        }
       
        public bool ConfigureCamera()
        {
            if (_index > -1 && _index < DllImport.GetNumberOfConnectedWebCams() && _isavailable)
                return DllImport.ConfigureCameraProperties(_index);
            else
                return false;
        }
        public float FrameRate() { return _frameRate; }

        //-----------------------------------------------------------------------------------------------------------------------------
        //Members:
        private string _name;
        private string _path;
        private int _index;
        private bool _isavailable;
        Thread worker;
        ManualResetEvent stopSignal;
        private int _width;
        private int _height;
        public event PropertyChangedEventHandler PropertyChanged;
        private event EventHandler BitmapReady;
        System.Diagnostics.Stopwatch timer = System.Diagnostics.Stopwatch.StartNew();
        double _framesProcessed;
        float _frameRate;
        private byte[] rawBuffer;

        public byte[] ImageBuffer
        {
            get { return rawBuffer; }
        }


        void SetBuffer()
        {
            rawBuffer = null;
            rawBuffer = new byte[_width * _height * 3];
         
        }
        void ComputeFrameRate()
        {
            _framesProcessed++;
            if (timer.ElapsedMilliseconds >= 1000)
            {
                _frameRate = (float)Math.Round(_framesProcessed * 1000.0 / timer.ElapsedMilliseconds);
                timer.Reset();
                timer.Start();
                _framesProcessed = 0;
            }
        }

        public override event ImageUpdateHandler OnImageUpdated;
        void OnBitmapReady(object sender, EventArgs e)
        {
            if (OnImageUpdated != null)
            {
               OnImageUpdated(this, new EventArgs());
            }
        }



        void RunWorker()
        {
            while (!stopSignal.WaitOne(0, true))
            {
                //1. get the byte buffer from the webcam 
                if (DllImport.GrabFrameWebcam(_index, rawBuffer, _width, _height))
                {
                    // signal that the mapped video memory is ready with a new buffer, causing the 'Source' to refresh with the new buffer...
                    OnBitmapReady(this, null);
                    Thread.Sleep(30); //increasing this will reduce the CPU load... reducing this is not recommended, as buffers will overlap
                }
            }
        }
        void Release()
        {
            worker = null;
            stopSignal.Close();
            stopSignal = null;
        }
        public bool IsActive
        {
            get
            {
                if (worker != null)
                {
                    if (worker.Join(0) == false)
                        return true;
                    Release();
                }
                return false;
            }
        }
        
        public void Dispose()
        {
            Stop();
        }
    
    }
}
