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
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Drawing;
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
        
        private int index_ = instances++;       // should be incremented on instantiation
        public int index
        {
            get { return index_;}
        }

        private CameraInfo info;
        public override CameraInfo Info
        {
            get
            {
                //throw new NotImplementedException();
                return info; 
            }    
        }

        public CameraSettings settings;
        public bool is_initialized = false;      
        public int bufSize;
        public byte[] imageBuffer;
        public object thread_locker = new object();
        public override event ImageUpdateHandler OnImageUpdated;
        // update rate (ms) with which the camera thread requests new images
        public int frameInterval;
        // store LOOP_DURATION ms of footage in the past/future
        public  const int LOOP_DURATION = 30000; 
        // calculate amount of storage needed for the given duration 
        private int LOOP_BUFFER_MAX_LENGTH = LOOP_DURATION / CameraSettings.DEFAULTS().updateRate_ms;
        // fifo buffer that stores last x images
        private Queue<BitmapTimestamp> loop_buffer = new Queue<BitmapTimestamp>();
        // check for thread termination   
        private ManualResetEvent grabFramesReset;
        #endregion

        #region Contstructor / Destructor
        public Camera(CameraInfo info, CameraSettings settings)
        {
            this.info = info;
            this.settings = settings;
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
        public List<BitmapTimestamp> buffer_items
        {
            get
            {
                List<BitmapTimestamp> items_out = new List<BitmapTimestamp>();
                List<BitmapTimestamp> buffer_items;

                // critical section: don't let the read thread dispose of bitmaps before we copy them first
                lock (thread_locker)
                {
                    buffer_items = loop_buffer.ToList<BitmapTimestamp>();

                    // return ;
                    foreach (BitmapTimestamp b in buffer_items)
                    {
                        items_out.Add(b.Clone() as BitmapTimestamp);
                    }
                }
                return items_out;
            }
        }

        /// <summary>
        /// Returns latest image as a Bitmap
        /// </summary>
        /// <returns>Bitmap containing the image data</returns>
        public override Bitmap ImageAsBitmap()
        {
            try
            {
                Bitmap backBuffer = new Bitmap(320, 240, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, 320, 240);

                // get at the bitmap data in a nicer way
                System.Drawing.Imaging.BitmapData bmpData =
                    backBuffer.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                    backBuffer.PixelFormat);

                int bufSize = this.bufSize;
                IntPtr ImgPtr = bmpData.Scan0;

                // grab the image
                lock (thread_locker)
                {
                    // Copy the RGB values back to the bitmap
                    System.Runtime.InteropServices.Marshal.Copy(this.imageBuffer, 0, ImgPtr, bufSize);
                }
                // Unlock the bits.
                backBuffer.UnlockBits(bmpData);
                return backBuffer;
            }
            catch (ArgumentException ex)
            {
                Logger.WriteLine("ArgumentException: " + ex);
                return new Bitmap(320, 240);      
            }
        }

        #endregion            

        /// <summary>
        /// Initializes the camera via the videoinput lib
        /// </summary>
        private void InitializeCaptureDevice(CameraInfo info, CameraSettings settings)
        {
            is_initialized = false;
            try
            {
                OpenCVDLL.OpenCVBindings.InitCapture(info.deviceId, settings.resX, settings.resY);
                bufSize = OpenCVDLL.OpenCVBindings.GetImageSize();
                imageBuffer = new byte[bufSize];
                frameInterval = CameraSettings.DEFAULTS().updateRate_ms;

                // StartFrameGrabbing();
                is_initialized = true;
            }
            catch (Exception e)
            {
                Logger.WriteLine("exception during camera init\n" + e.ToString());
            }
        }

        #region Frame Grabbing Start/Stop
      
        public override void StartFrameGrabbing()
        {
            if (!is_initialized)
            {
                InitializeCaptureDevice(info, settings);
            }

            if (!this.running)
            {        
                //grabFrm = new Thread(GrabFrames);
                try
                {
                    Logger.WriteLine("Starting Frame Grabber");
                    //grabFrm.Start();
                    // TODO: run this code in the ThreadPool
                    grabFramesReset = new ManualResetEvent(false); 
                    ThreadPool.QueueUserWorkItem((object o) => { GrabFrames_Thread(); });
                    Thread.Sleep(200);
                }
                catch (Exception e)
                {
                    Logger.WriteLine("Exception while trying to start Framegrab");
                    Logger.WriteLine(e.ToString());
                }
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
            Logger.WriteLine(index + " TryStopFrameGrabbing");
            if (this.OnImageUpdated == null)
            {
                Logger.WriteLine(" shutting down camera");
                //  Stop device
                StopGrabbing();
                this.running = false;           
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
            if (is_initialized)
            {
                is_initialized = false;
                running = false;
                // Wait for frame grab thread to end or 500ms timeout to elapse
                grabFramesReset.WaitOne(500);
                try
                {
                    OpenCVDLL.OpenCVBindings.StopCamera(this.info.deviceId);
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
                Logger.WriteLine(index + " GrabFrames");

                lock (thread_locker)
                {
                    foreach (BitmapTimestamp b in loop_buffer)
                    {
                        b.Dispose();
                    }
                }

                // purge the video buffer when starting frame grabbing
                // we don't need the previously recorded and potentially very old bitmaps in the buffer
                lock (thread_locker)
                {
                    loop_buffer.Clear();
                }
                running = true;
                while (running)
                {
                    //Logger.WriteLine("Camera: Update!");
                    lock (thread_locker)
                    {
                        unsafe
                        {
                            fixed (byte* ptr = imageBuffer)
                            {
                                try
                                {
                                    OpenCVDLL.OpenCVBindings.GetNextFrame(this.index, ptr);
                                }
                                catch (Exception e)
                                {
                                    Logger.WriteLine("Cam Exception " + e);
                                    // shutdown cam
                                    running = false;
                                }
                            }
                        }
                    }

                    lock (thread_locker)
                    {
                        if (loop_buffer.Count < LOOP_BUFFER_MAX_LENGTH)
                        {
                            loop_buffer.Enqueue(new BitmapTimestamp(ImageAsBitmap()));
                        }
                        else
                        {
                            BitmapTimestamp destroyMe = loop_buffer.Dequeue();
                            destroyMe.Dispose();
                            loop_buffer.Enqueue(new BitmapTimestamp(ImageAsBitmap()));
                        }
                    }

                    if (running)
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
                running = false;
                // signal termination of this thread 
                grabFramesReset.Set();
            }
        }

        public override string ToString()
        {
            return this.info.FriendlyName();
        }

        #region IBufferingImager Methods
        /// <summary>
        /// IBufferingImager Method -- return index image from end of queue 
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public Bitmap GetBufferItemAtIndex(int index)
        {
            int tail_idx = loop_buffer.Count - 1 - index;
            if (tail_idx > 0 && tail_idx < loop_buffer.Count)
            {
                return loop_buffer.ElementAt(tail_idx).image;
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
            int tail_idx = loop_buffer.Count - 1 - index;
            if (tail_idx < loop_buffer.Count)
            {
                return loop_buffer.ElementAt(tail_idx).timeStamp;
            }
            else
            {
                return DateTime.MinValue;
            }
        }
    }
        #endregion
}
