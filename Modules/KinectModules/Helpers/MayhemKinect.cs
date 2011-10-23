using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Research.Kinect.Nui;

namespace KinectModules.Helpers
{
    public class MayhemKinect
    {
        private static Runtime nui; 

        private static MayhemKinect instance;

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

        public static void AttachSkeletonEventHandler (EventHandler<SkeletonFrameReadyEventArgs> handler)
        {
            nui.SkeletonFrameReady -= handler;
            nui.SkeletonFrameReady += handler; 
        }

        public static void DetachSkeletonEventHandler (EventHandler<SkeletonFrameReadyEventArgs> handler)
        {
            nui.SkeletonFrameReady -= handler; 
        }

        public static void AttachDepthFrameEventHandler(EventHandler<ImageFrameReadyEventArgs> handler)
        {
            nui.DepthFrameReady -= handler;
            nui.DepthFrameReady += handler; 
        }

        public static void DetachDepthFrameEventHandler(EventHandler<ImageFrameReadyEventArgs> handler)
        {
            nui.DepthFrameReady -= handler; 
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
