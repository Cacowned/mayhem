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
        private static int instances = 0; 

        private int index_ = instances++;       // should be incremented on instantiation

        public int index
        {
            get { return index_;}
        }

        public CameraInfo info;
        public CameraSettings settings;

        public bool is_initialized = false;
        

        public int bufSize;
        public byte[] imageBuffer;

        public object thread_locker = new object();

        public override event ImageUpdateHandler OnImageUpdated;

        // (ms) : update rate with which the camera thread requests new images
        public int frameInterval;

        private Thread grabFrm = null;

        // store LOOP_DURATION ms of footage in the past/future
        public static readonly int LOOP_DURATION = 30000; 

        // calculate amount of storage needed for the given duration 
        private int LOOP_BUFFER_MAX_LENGTH = LOOP_DURATION / CameraSettings.DEFAULTS().updateRate_ms;

        // fifo buffer that stores last x images
        private Queue<BitmapTimestamp> loop_buffer = new Queue<BitmapTimestamp>();
        


        public Camera(CameraInfo info, CameraSettings settings)
        {
            this.info = info;
            this.settings = settings;
        }

     /// <summary>
     /// Returns latest image as a Bitmap
     /// </summary>
     /// <returns>Bitmap containing the image data</returns>
        public override Bitmap ImageAsBitmap()
        {
            //int stride = 320 * 3;
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

        public override void StartFrameGrabbing()
        {
            if (!is_initialized)
            {
                InitializeCaptureDevice(info, settings);
            }

            grabFrm = new Thread(GrabFrames);
            try
            {
                Logger.WriteLine("Starting Frame Grabber");
                grabFrm.Start();
                this.running = true;
            }
            catch (Exception e)
            {
                Logger.WriteLine("Exception while trying to start Framegrab");
                Logger.WriteLine(e.ToString());
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
            is_initialized = false;
            running = false;
            // Wait for frame grab thread to end or 500ms timeout to elapse
            if (grabFrm.IsAlive)
                grabFrm.Join(500);
            OpenCVDLL.OpenCVBindings.StopCamera(this.info.deviceId);

        }

        private void GrabFrames()
        {
            Logger.WriteLine(index + " GrabFrames");
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
                
               
                // add an item to the queue buffer
                BitmapTimestamp b = new BitmapTimestamp(ImageAsBitmap());

                if (loop_buffer.Count < LOOP_BUFFER_MAX_LENGTH)
                {
                     loop_buffer.Enqueue(b);
                }
                else
                {                   
                    loop_buffer.Dequeue();                  // gc should handle the spurious bitmap                             
                    loop_buffer.Enqueue(b);
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
            Logger.WriteLine("GrabFrame Thread terminated");
            running = false;
        }

        public void Release()
        {
            running = false;
            OpenCVDLL.OpenCVBindings.StopCamera(this.info.deviceId);
            is_initialized = false;
        }


        public override string ToString()
        {
            return this.info.FriendlyName();
        }

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

    // TODO: Add a dummy camera (in case no real device is attached!) 
}
