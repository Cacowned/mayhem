using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows;
using System.Drawing;
using System.Windows.Interop;
using System.IO;
using System.Windows.Media.Imaging;
using System.Threading;

namespace DefaultModules.WebcamHelpers
{
    public class Webcam
    {
        /// <summary>
        /// Device to capture from
        /// </summary>
        private Device _device;

        /// <summary>
        /// Captures images ;)
        /// </summary>
        private ImageCapture _capture;

        /// <summary>
        /// Indicates if capturing is active right now
        /// </summary>
        private volatile bool _started = false;

        /// <summary>
        /// Is set by Capture() and filled by CaptureThread
        /// </summary>
        private Bitmap _captureImage = null;

        public List<Bitmap> bitmaps;
        private object bitmapLock;

        /// <summary>
        /// Gets or Sets the current capture device
        /// </summary>
        public Device Device
        {
            get { return _device; }
            set
            {
                if (_capture != null)
                {
                    Stop();
                    _capture.Dispose();
                    _capture = null;
                }

                _device = value;

                if (_device != null)
                {
                    _capture = new ImageCapture(_device);
                    _capture.Initialize();
                }
            }
        }

        public Webcam()
        {
            bitmaps = new List<Bitmap>(60);
            bitmapLock = new object();
        }

        /// <summary>
        /// Starts the image capturer
        /// </summary>
        public void Start()
        {
            if (_started == false)
            {
                _started = true;
                _capture.Start();
                ThreadPool.QueueUserWorkItem(new WaitCallback(CaptureThread));
            }
        }

        /// <summary>
        /// Stops the capturer
        /// </summary>
        public void Stop()
        {
            _started = false;
            _capture.Pause();
        }

        /// <summary>
        /// Processes the captured images and paints them on the control
        /// </summary>
        /// <param name="state"></param>
        private void CaptureThread(object state)
        {
            while (_started)
            {

                using (ComBitmap currentImage = _capture.Image)
                {

                    lock (bitmapLock) {
                        if (bitmaps.Count >= 60) {
                            bitmaps.RemoveAt(0);
                        }

                        _captureImage = new Bitmap(_capture.OriginalWidth, _capture.OriginalHeight);
                        Graphics captureG = Graphics.FromImage(_captureImage);
                        captureG.DrawImage(currentImage.Bitmap,
                            new Rectangle(0, 0, _captureImage.Width, _captureImage.Height),
                            new Rectangle(0, 0, currentImage.Bitmap.Width, currentImage.Bitmap.Height), GraphicsUnit.Pixel);

                        Bitmap captureImage = _captureImage;

                        _captureImage = null;
                        bitmaps.Add(captureImage);
                        
                        captureG.Dispose();
                    }
                    
                    Thread.Sleep(1000);

                }
            }

        }


        /// <summary>
        /// Captures the current image
        /// </summary>
        /// <returns></returns>
        public Bitmap Capture()
        {
            lock (bitmapLock) {
                if (bitmaps.Count <= 0)
                    return null;

                return bitmaps[bitmaps.Count - 1];
            }
        }
    }
}
