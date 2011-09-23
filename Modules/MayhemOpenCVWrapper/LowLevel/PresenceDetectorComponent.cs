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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using MayhemCore;

namespace MayhemOpenCVWrapper.LowLevel
{
    public class PresenceDetectorComponent : IVisionEventComponent
    {
        private OpenCVDLL.PresenceDetector pd;

        public delegate void DetectionHandler(object sender, Point[] points);
        public event DetectionHandler OnPresenceUpdate;

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

        public override void update_frame(object sender, EventArgs e)
        {
            Camera camera = sender as Camera;

            // copy over image data to the DLL 
            try
            {
                lock (camera.thread_locker)
                {
                    unsafe
                    {
                        fixed (byte* ptr = camera.imageBuffer)
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
