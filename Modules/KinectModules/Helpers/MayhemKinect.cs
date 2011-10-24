using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Kinect.Nui;

namespace KinectModules.Helpers
{
    /// <summary>
    /// Provides a single Kinect nui instance to Mayhem Kinect Modules
    /// </summary>
    public class MayhemKinect
    {
        private static Runtime nui; 

        private static MayhemKinect instance;

        /// <summary>
        /// Returns the NUI runtime instance
        /// </summary>
        public static MayhemKinect Instance 
        {
            get
            {
                // initialize the instance the first time the property is accessed
                if (instance == null)
                    instance = new MayhemKinect();
                return instance;
            }
        }

        /// <summary>
        /// Access NUI SkeletonEngine
        /// </summary>
        public SkeletonEngine SkeletonEngine
        {
            get { return nui.SkeletonEngine; }
        }

        public Camera NuiCamera
        {
            get { return nui.NuiCamera;  }
        }


        /// <summary>
        /// Atach SkeletonEventHandler to Runtime
        /// </summary>
        /// <param name="handler"></param>
        public void AttachSkeletonEventHandler (EventHandler<SkeletonFrameReadyEventArgs> handler)
        {
            nui.SkeletonFrameReady -= handler;
            nui.SkeletonFrameReady += handler; 
        }

        /// <summary>
        /// Detach SekeltonEventHandler from Runtime
        /// </summary>
        /// <param name="handler"></param>
        public void DetachSkeletonEventHandler (EventHandler<SkeletonFrameReadyEventArgs> handler)
        {
            nui.SkeletonFrameReady -= handler; 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
         void AttachDepthFrameEventHandler(EventHandler<ImageFrameReadyEventArgs> handler)
        {
            nui.DepthFrameReady -= handler;
            nui.DepthFrameReady += handler; 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
         void DetachDepthFrameEventHandler(EventHandler<ImageFrameReadyEventArgs> handler)
        {
            nui.DepthFrameReady -= handler; 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="handler"></param>
         void AttachVideoFrameEventHandler(EventHandler<ImageFrameReadyEventArgs> handler)
        {
            nui.VideoFrameReady -= handler;
            nui.VideoFrameReady += handler; 
        }

         void DetachVideoFrameEventHandler(EventHandler<ImageFrameReadyEventArgs> handler)
        {
            nui.VideoFrameReady -= handler; 
        }



        private MayhemKinect()
        {
            nui = new Runtime(); 

            try
            {
                nui.Initialize(RuntimeOptions.UseDepthAndPlayerIndex | RuntimeOptions.UseSkeletalTracking |
                               RuntimeOptions.UseColor);
            }
            catch (InvalidOperationException)
            {
                System.Windows.MessageBox.Show("Runtime initialization failed. Please make sure Kinect device is plugged in.");
                return;
            }
        }
        

        
    }
}
