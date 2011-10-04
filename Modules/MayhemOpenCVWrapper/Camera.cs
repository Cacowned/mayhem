/*
 *  Camera.cs
 * 
 *  Package functionality of an an individual camera and abstracts the camera functions for use by
 *  Mayhem Modules. 
 * 
 *  (c) 2010/2011, Microsoft Applied Sciences Group
 *  
 *  Author: Sven Kratz
 * 
 */

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using MayhemCore;

namespace MayhemOpenCVWrapper
{
    /// <summary>
    /// This class packages the functions for individual camera devices
    /// Modules using vision will now have the ability to request image updates from
    /// multiple cameras.
    /// 
    /// This class will provide the update events. 
    /// </summary>
    public class Camera : ImagerBase, IBufferingImager
    {
        #region Fields and Properties
        private static int instances = 0;       // static counter of camera instances intialized
        
        private int index = instances++;       // should be incremented on instantiation

        public static readonly double LOOP_BUFFER_UPDATE_MS = 250.0;  // update the loop only every quarter second -- this should be sufficient for the Picture Event
        private DateTime loopBufferLastUpdate = DateTime.Now;

        private VideoDiskBuffer videoDiskBuffer = new VideoDiskBuffer(); 

        public int Index
        {
            get { return index;}
        }

        public override bool Running
        {
            get;
            protected set;
        }

        public override CameraInfo Info
        {
            get;
            protected set;
        }

        public override CameraSettings Settings
        {
            get;
            protected set; 
        }
        public bool IsInitialized = false;      
        public int BufferSize;
        public byte[] ImageBuffer;
        public object ThreadLocker = new object();
        public override event ImageUpdateHandler OnImageUpdated;
        // update rate (ms) with which the camera thread requests new images
        public int FrameInterval;
        // store LOOP_DURATION ms of footage in the past/future
        public const int LoopDuration = 30000; 
        // calculate amount of storage needed for the given duration 
        private int loopBufferMaxLength = LoopDuration / CameraSettings.Defaults().UpdateRateMs;
        // fifo buffer that stores last x images
        private Queue<BitmapTimestamp> loopBuffer = new Queue<BitmapTimestamp>();
        // check for thread termination   
        private ManualResetEvent grabFramesReset;
        #endregion

        #region Constructor / Destructor
        public Camera(CameraInfo info, CameraSettings settings)
        {
            this.Info = info;
            this.Settings = settings;
            grabFramesReset = new ManualResetEvent(false);
        }

        ~Camera()
        {
            Logger.WriteLine("dtor");
            StopGrabbing();
        }
        #endregion

        #region Bitmap Export

        /// <summary>
        /// Return copies all loop buffer items
        /// </summary>
        public List<BitmapTimestamp> BufferItems
        {
            get
            {
                List<BitmapTimestamp> itemsOut = new List<BitmapTimestamp>();

                // critical section: don't let the read thread dispose of bitmaps before we copy them first
                lock (ThreadLocker)
                {
                    List<BitmapTimestamp> bufferItems = loopBuffer.ToList<BitmapTimestamp>();

                    // return ;
                    foreach (BitmapTimestamp b in bufferItems)
                    {
                        itemsOut.Add(b.Clone() as BitmapTimestamp);
                    }
                }
                return itemsOut;
            }
        }

        public List<Bitmap> videoDiskBufferItems
        {
            get
            {
                return videoDiskBuffer.RetrieveBitmapsFromDisk();
            }
        }

        /// <summary>
        /// Returns latest image as a Bitmap
        /// </summary>
        /// <returns>Bitmap containing the image data</returns>
        public override Bitmap ImageAsBitmap()
        {
            int w = this.Settings.ResX;
            int h = this.Settings.ResY; 
            try
            {
                Bitmap backBuffer = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, w, h);

                // get at the bitmap data in a nicer way
                System.Drawing.Imaging.BitmapData bmpData =
                    backBuffer.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                    backBuffer.PixelFormat);

                int bufSize = this.BufferSize;
                IntPtr imgPtr = bmpData.Scan0;

                // grab the image
                lock (ThreadLocker)
                {
                    // Copy the RGB values back to the bitmap
                    System.Runtime.InteropServices.Marshal.Copy(this.ImageBuffer, 0, imgPtr, bufSize);
                }
                // Unlock the bits.
                backBuffer.UnlockBits(bmpData);
                return backBuffer;
            }
            catch (ArgumentException ex)
            {
                Logger.WriteLine("ArgumentException: " + ex);
                return new Bitmap(w, h);      
            }
        }

        #endregion            

        /// <summary>
        /// Initializes the camera via the videoinput lib
        /// </summary>
        private void InitializeCaptureDevice(CameraInfo info, CameraSettings settings)
        {
            Logger.WriteLine("========= CAM: " + info.DeviceId+ " ======================");
            try
            {
                // lock on the CameraDriver to prevent multiple simultaneous calls to InitCapture
                lock (CameraDriver.Instance)
                {
                    OpenCVDLL.OpenCVBindings.InitCapture(info.DeviceId, settings.ResX, settings.ResY);
                }
                BufferSize = OpenCVDLL.OpenCVBindings.GetImageSize();
                ImageBuffer = new byte[BufferSize];
                FrameInterval = CameraSettings.Defaults().UpdateRateMs;

                // StartFrameGrabbing();
                IsInitialized = true;
            }
            catch (Exception e)
            {
                Logger.WriteLine("exception during camera init\n" + e.ToString());
            }
        }

        #region Frame Grabbing Start/Stop
      
        public override void StartFrameGrabbing()
        {
            if (!IsInitialized)
            {
                try
                {
                    InitializeCaptureDevice(Info, Settings);
                }
                catch (AccessViolationException avEx)
                {
                    Logger.WriteLine("Access Violation Exception when initializing camera: " + Info + "\n" + avEx);
                }
            }

            if (!this.Running)
            {
                //grabFrm = new Thread(GrabFrames);
                try
                {
                    Logger.WriteLine("Starting Frame Grabber");
                    ThreadPool.QueueUserWorkItem((object o) => { GrabFrames_Thread(); });           
                }
                catch (Exception e)
                {
                    Logger.WriteLine("Exception while trying to start Framegrab");
                    Logger.WriteLine(e.ToString());
                }
            }
            else
            {
                Logger.WriteLine("StartFrameGrabbing(): Camera Running -- ignore");
            }
        }

        // TODO: override the event handler and just add this to the process of 
        // removing handlers
        // i.e. if this.OnImageUpdates == null after
        // OnImageUpdate-= blah, execute StopGrabbing()

        /// <summary>
        /// This method will try to deactivate the camera.
        /// The camera will be deactivated if there are no handlers left
        /// attached to OnImageUpdated
        /// </summary>
        /// <returns>Success of the deactivation procedure.</returns>
        public override bool TryStopFrameGrabbing()
        {
            Logger.WriteLine(Index + " TryStopFrameGrabbing");
            if (this.OnImageUpdated == null)
            {
                Logger.WriteLine(" shutting down camera");
                //  Stop device
                StopGrabbing();
                this.Running = false;  
         
                // clear disk video frame buffer
                videoDiskBuffer.ClearAndResetBuffer();

                return true;
            }
            else
            {
                Logger.WriteLine(" handlers still attached, not shutting down camera");
                return false;
            }
        }

        private void StopGrabbing()
        {
            if (IsInitialized)
            {
                IsInitialized = false;
                Running = false;
                // Wait for frame grab thread to end or 500ms timeout to elapse
                grabFramesReset.WaitOne(500);
                try
                {
                    OpenCVDLL.OpenCVBindings.StopCamera(this.Info.DeviceId);
                }
                catch (Exception ex)
                {
                    Logger.WriteLine("Exception While Shutting Down Cam: " + ex);
                }

                Thread.Sleep(200);
            }
        }
        #endregion

        /// <summary>
        /// Thread that updates the frame buffer periodically from videoInputLib via OpenCVDLL 
        /// </summary>
        private void GrabFrames_Thread()
        {
            try
            {
                Logger.WriteLine(Index + " GrabFrames");

                lock (ThreadLocker)
                {
                    foreach (BitmapTimestamp b in loopBuffer)
                    {
                        b.Dispose();
                    }
                }

                // purge the video buffer when starting frame grabbing
                // we don't need the previously recorded and potentially very old bitmaps in the buffer
                lock (ThreadLocker)
                {
                    loopBuffer.Clear();
                }
                Running = true;
                while (Running)
                {
                    //Logger.WriteLine("Camera: Update!");
                    lock (ThreadLocker)
                    {
                        unsafe
                        {
                            fixed (byte* ptr = ImageBuffer)
                            {
                                try
                                {
                                    OpenCVDLL.OpenCVBindings.GetNextFrame(this.Index, ptr);
                                }
                                catch (Exception e)
                                {
                                    Logger.WriteLine("Cam Exception " + e);
                                    // shutdown cam
                                    Running = false;
                                }
                            }
                        }
                    }

                    lock (ThreadLocker)
                    {
                        DateTime now = DateTime.Now; 
                        TimeSpan last_update  = now - this.loopBufferLastUpdate;                   
                        if ( last_update.TotalMilliseconds >= LOOP_BUFFER_UPDATE_MS)
                        {
                            this.loopBufferLastUpdate = DateTime.Now;
                            if (loopBuffer.Count < loopBufferMaxLength)
                            {
                                loopBuffer.Enqueue(new BitmapTimestamp(ImageAsBitmap()));
                            }
                            else
                            {
                                BitmapTimestamp destroyMe = loopBuffer.Dequeue();
                                destroyMe.Dispose();
                                loopBuffer.Enqueue(new BitmapTimestamp(ImageAsBitmap()));
                            }
                        }
                    }

                    //// Experimental
                    videoDiskBuffer.AddBitmap(ImageAsBitmap());
                    ////

                    if (Running)
                    {
                        Thread.Sleep(FrameInterval);
                    }
                    else
                    {
                        break;
                    }

                    if (OnImageUpdated != null)
                    {
                        // Logger.Write("Camera: Sending new Frame to listeners!");
                        OnImageUpdated(this, new EventArgs());
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine("GrabFrame Thread Exception:\n" + ex);
            }
            finally
            {
                Logger.WriteLine("GrabFrame Thread terminated normally");
                Running = false;
                // signal termination of this thread 
                grabFramesReset.Set();
            }
        }

        public override string ToString()
        {
            return this.Info.FriendlyName();
        }

        #region IBufferingImager Methods
        /// <summary>
        /// IBufferingImager Method -- return index image from end of queue 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Bitmap GetBufferItemAtIndex(int index)
        {
            int tailIdx = loopBuffer.Count - 1 - index;
            if (tailIdx > 0 && tailIdx < loopBuffer.Count)
            {
                return loopBuffer.ElementAt(tailIdx).Image;
            }
            else
            {
                return null; 
            }
        }

        /// <summary>
        /// IBufferingImager Method
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public DateTime GetBufferTimeStampAtIndex(int index)
        {
            int tailIdx = loopBuffer.Count - 1 - index;
            if (tailIdx < loopBuffer.Count)
            {
                return loopBuffer.ElementAt(tailIdx).TimeStamp;
            }
            else
            {
                return DateTime.MinValue;
            }
        }
    }
        #endregion
}
