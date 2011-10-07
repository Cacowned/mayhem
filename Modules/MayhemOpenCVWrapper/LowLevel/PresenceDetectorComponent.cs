/*
 * PresenceDetectorComponent.cs
 * 
 * Low level component interfacing with the OpenCVDLL presence detector component. 
 * 
 * (c) 2011, Microsoft Applied Sciences Group
 * 
 * Author: Sven Kratz
 * 
 */

using System;
using System.Drawing;
using MayhemCore;

namespace MayhemOpenCVWrapper.LowLevel
{
    public class PresenceDetectorComponent : CameraImageListener
    {
        public static double DEFAULT_SENSITIVITY
        {
            get
            {
                return OpenCVDLL.PresenceDetector.DEFAULT_SENSITIVITY; 
            }
        }

        private OpenCVDLL.PresenceDetector pd;

        public delegate void DetectionHandler(object sender, Point[] points);
        public event DetectionHandler OnPresenceUpdate;

        /// <summary>
        /// Sets the decay sensitivity of the presence detector
        /// Basically this determines how long the presence is still valid when no movement has
        /// been detected. 
        /// </summary>
        public double Sensitivity
        {
            get
            {
                if (pd != null)
                {
                    return pd.Sensitivity;
                }
                else
                {
                    return 0;
                }
            }
            set
            {
                if (pd != null)
                {
                    pd.Sensitivity = value; 
                }
            }
        }

        private bool presence_ = false;
        public bool presence
        {
            get { return presence_; }
        }

        public PresenceDetectorComponent(int width, int height)
        {
            pd = new OpenCVDLL.PresenceDetector(width, height);
        }

        ~PresenceDetectorComponent()
        {
            Logger.WriteLine("dtor");
            pd.Dispose();
        }

        public override void UpdateFrame(object sender, EventArgs e)
        {
            Camera camera = sender as Camera;

            // copy over image data to the DLL 
            try
            {
                lock (camera.ThreadLocker)
                {
                    unsafe
                    {
                        fixed (byte* ptr = camera.ImageBuffer)
                        {
                            pd.ProcessFrame(ptr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLine("Frame Grab Exception!\n" + ex);
            }

            // find out what the results are
            presence_ = pd.GetCurrentPresence();
            // TODO.....
            Point[] points = null;
            // .... 

            if (OnPresenceUpdate != null && pd.IsInitialized)
            {
                OnPresenceUpdate(this, points);
            }
        }
    }
}
