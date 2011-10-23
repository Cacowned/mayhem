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
    /// Modules using vision have the ability to request image updates from
    /// multiple cameras.
    /// 
    /// The camera maintains a 30s disk buffer to enable the generation of video files from sequences of camera frames
    /// as well as a 30s, 4 frame-per-second in-memory image buffer for capturing still shots or similar tasks. 
    /// </summary>
    public sealed class Camera : ImagerBase, IBufferingImager, IDisposable
    {
        #region Fields and Properties

        private static const double KLoopBufferUpdateMs = 250.0; 

        // update the loop only every quarter second -- this should be sufficient for the Picture Event       
        public static double LoopBufferUpdateMs
        {
            get { return KLoopBufferUpdateMs; }
        }

        // store LOOP_DURATION ms of footage in the past/future
        private static const int KLoopDuration = 30000;

        public static int LoopDuration
        {
            get { return KLoopDuration; }
        }

        // Image update event handler. 
        public override event ImageUpdateHandler OnImageUpdated;

        /// <summary>
        /// Return index of camera device
        /// </summary>
        public int Index
        {
            get { return index; }
        }

        /// <summary>
        /// The Camera's running state.
        /// </summary>
        public override bool Running
        {
            get;
            protected set;
        }

        /// <summary>
        /// The camera info returned from VideoInput. 
        /// </summary>
        public override CameraInfo Info
        {
            get;
            protected set;
        }

        /// <summary>
        /// The setting with which the camera was initialized. 
        /// </summary>
        public override CameraSettings Settings
        {
            get;
            protected set;
        }

        /// <summary>
        /// The buffer storing the current camera image.
        /// Access is thread-safe. 
        /// </summary>
        public byte[] ImageBuffer
        {
            get
            {
                lock (threadLocker)
                {
                    return imageBuffer;
                }
            }

            private set
            {
                imageBuffer = value;
            }
        }

        /// <summary>
        /// The size in bytes of the camera buffer. 
        /// </summary>
        public int BufferSize
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets or sets the camera's ability to record video 
        /// </summary>
        public bool IsRecordingVideo
        {
            get
            {
                return recordingVideo;
            }

            set
            {
                if (value)
                {
                    videoDiskBuffer.ClearAndResetBuffer();
                    recordingVideo = true;
                }
                else
                {
                    recordingVideo = false;
                }
            }
        }

        private byte[] imageBuffer;

        // lock object on the image update thread 
        private readonly object threadLocker;

        // Camera initialization state.
        private bool isInitialized;

        // static counter of camera instances initialized
        private static int instances;

        // increment on instantiation 
        private readonly int index;
        private DateTime loopBufferLastUpdate;
        private readonly VideoDiskBuffer videoDiskBuffer;

        // update rate (ms) with which the camera thread requests new images
        private int frameInterval;

        // calculate amount of storage needed for the given duration 
        private readonly int loopBufferMaxLength;

        // fifo buffer that stores last x images
        private readonly Queue<BitmapTimestamp> loopBuffer;

        // check for thread termination   
        private readonly AutoResetEvent grabFramesReset;

        // status of video recording
        private bool recordingVideo;

        #endregion

        #region Constructor / Destructor
        /// <summary>
        /// Camera object constructor
        /// </summary>
        /// <param name="info"> CameraInfo object, containing camera metadata </param>
        /// <param name="settings">CameraSettings object, containing settings for the camera</param>
        public Camera(CameraInfo info, CameraSettings settings)
        {
            Info = info;
            Settings = settings;

            threadLocker = new object();

            index = instances++;
            loopBufferLastUpdate = DateTime.Now;
            loopBufferMaxLength = LoopDuration / CameraSettings.Defaults().UpdateRateMs;
            loopBuffer = new Queue<BitmapTimestamp>();

            videoDiskBuffer = new VideoDiskBuffer();

            grabFramesReset = new AutoResetEvent(false);
        }

        /// <summary>
        /// Destructor. Last chance to stop the camera if it is running. 
        /// </summary>
        ~Camera()
        {
            StopGrabbing();
        }

        /// <summary>
        /// Returns string representation of Camera object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Info.ToString();
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
                lock (threadLocker)
                {
                    List<BitmapTimestamp> bufferItems = loopBuffer.ToList();

                    // return ;
                    foreach (BitmapTimestamp b in bufferItems)
                    {
                        itemsOut.Add(b.Clone() as BitmapTimestamp);
                    }
                }

                return itemsOut;
            }
        }

        /// <summary>
        /// Returns item from the on-disk video frame cache. 
        /// </summary>
        public List<Bitmap> VideoDiskBufferItems
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
            int w = Settings.ResX;
            int h = Settings.ResY;
            try
            {
                Bitmap backBuffer = new Bitmap(w, h, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                Rectangle rect = new Rectangle(0, 0, w, h);

                // get at the bitmap data in a nicer way
                System.Drawing.Imaging.BitmapData bmpData =
                    backBuffer.LockBits(
                        rect,
                        System.Drawing.Imaging.ImageLockMode.ReadWrite,
                        backBuffer.PixelFormat);

                IntPtr imgPtr = bmpData.Scan0;

                // grab the image
                lock (threadLocker)
                {
                    // Copy the RGB values back to the bitmap
                    System.Runtime.InteropServices.Marshal.Copy(ImageBuffer, 0, imgPtr, BufferSize);
                }

                // Unlock the bits.
                backBuffer.UnlockBits(bmpData);

                // code analysis warning: backbuffer does not get disposed, but is returned
                return backBuffer;
            }
            catch (ArgumentException ex)
            {
                Logger.WriteLine("ArgumentException: " + ex);

                return new Bitmap(w, h);
            }
        }

        #endregion

        #region Internal

        /// <summary>
        /// Initializes the camera via the C++/CLI wrapper, which in turn accesses the videoinput lib
        /// </summary>
        private void InitializeCaptureDevice(CameraInfo info, CameraSettings settings)
        {
            Logger.WriteLine("========= CAM: " + info.DeviceId + " ======================");
            try
            {
                // lock on the CameraDriver to prevent multiple simultaneous calls to InitCapture
                lock (CameraDriver.Instance)
                {
                    OpenCVDLL.OpenCVBindings.InitCapture(info.DeviceId, settings.ResX, settings.ResY);
                }

                BufferSize = OpenCVDLL.OpenCVBindings.GetImageSize();
                ImageBuffer = new byte[BufferSize];
                frameInterval = CameraSettings.Defaults().UpdateRateMs;

                // StartFrameGrabbing();
                isInitialized = true;
            }
            catch (Exception e)
            {
                Logger.WriteLine("exception during camera init\n" + e);
            }
        }

        #region Frame Grabbing Start/Stop

        public override void StartFrameGrabbing()
        {
            if (!isInitialized)
            {
                try
                {
                    InitializeCaptureDevice(Info, Settings);
                }
                catch (AccessViolationException violation)
                {
                    Logger.WriteLine("Access Violation Exception when initializing camera: " + Info + "\n" + violation);
                }
            }

            if (!Running)
            {
                try
                {
                    recordingVideo = true;
                    Logger.WriteLine("Starting Frame Grabber");
                    ThreadPool.QueueUserWorkItem(o => GrabFrames());
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
            if (OnImageUpdated == null)
            {
                Logger.WriteLine(" shutting down camera");

                // Stop device
                StopGrabbing();
                Running = false;

                // clear disk video frame buffer
                videoDiskBuffer.ClearAndResetBuffer();

                return true;
            }
            
            Logger.WriteLine(" handlers still attached, not shutting down camera");
            return false;
        }

        private void StopGrabbing()
        {
            if (isInitialized)
            {
                isInitialized = false;
                Running = false;

                // Wait for frame grab thread to end or 500ms timeout to elapse
                if (grabFramesReset.WaitOne())
                {
                    try
                    {
                        OpenCVDLL.OpenCVBindings.StopCamera(Info.DeviceId);
                    }
                    catch (Exception ex)
                    {
                        Logger.WriteLine("Exception While Shutting Down Cam: " + ex);
                    }
                }
                else
                {
                    Logger.WriteLine("Problem Stopping Frame Grab Thread");
                }
            }
        }
        #endregion

        /// <summary>
        /// Thread that updates the frame buffer periodically from videoInputLib via OpenCVDLL 
        /// </summary>
        private void GrabFrames()
        {
            try
            {
                Logger.WriteLine(Index + " GrabFrames");

                lock (threadLocker)
                {
                    foreach (BitmapTimestamp b in loopBuffer)
                    {
                        b.Dispose();
                    }
                }

                // purge the video buffer when starting frame grabbing
                // we don't need the previously recorded and potentially very old bitmaps in the buffer
                lock (threadLocker)
                {
                    loopBuffer.Clear();
                }

                Running = true;
                while (Running)
                {
                    lock (threadLocker)
                    {
                        unsafe
                        {
                            fixed (byte* ptr = ImageBuffer)
                            {
                                try
                                {
                                    OpenCVDLL.OpenCVBindings.GetNextFrame(Index, ptr);
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

                    lock (threadLocker)
                    {
                        DateTime now = DateTime.Now;
                        TimeSpan lastUpdate = now - loopBufferLastUpdate;
                        if (lastUpdate.TotalMilliseconds >= LoopBufferUpdateMs)
                        {
                            loopBufferLastUpdate = DateTime.Now;
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

                    // Add new frame to the on-disk image cache
                    if (recordingVideo)
                        videoDiskBuffer.AddBitmap(ImageAsBitmap());

                    if (Running)
                    {
                        Thread.Sleep(frameInterval);
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

        #endregion

        #region IBufferingImager Methods
        /// <summary>
        /// IBufferingImager Method -- return index image from end of queue 
        /// </summary>
        /// <param name="index">Item Index</param>
        /// <returns></returns>
        public Bitmap GetBufferItemAtIndex(int index)
        {
            int tailIdx = loopBuffer.Count - 1 - index;
            if (tailIdx > 0 && tailIdx < loopBuffer.Count)
            {
                return loopBuffer.ElementAt(tailIdx).Image;
            }

            return null;
        }

        /// <summary>
        /// IBufferingImager Method
        /// </summary>
        /// <param name="index">Item Index</param>
        /// <returns></returns>
        public DateTime GetBufferTimeStampAtIndex(int index)
        {
            int tailIdx = loopBuffer.Count - 1 - index;
            if (tailIdx < loopBuffer.Count)
            {
                return loopBuffer.ElementAt(tailIdx).TimeStamp;
            }

            return DateTime.MinValue;
        }

        public void Dispose()
        {
            videoDiskBuffer.Dispose();
            grabFramesReset.Dispose();
        }
    }
        #endregion
}
