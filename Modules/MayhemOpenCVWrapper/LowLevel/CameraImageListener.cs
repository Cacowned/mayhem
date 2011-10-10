/*
 *   IVisionEventComponent
 * 
 *   Abstract base class for Event components that use vision. 
 *   (c) Microsoft Applied Sciences Group, 2011
 *   
 *   Author: Sven Kratz
 *   
 */ 
using System;
using MayhemCore;

namespace MayhemOpenCVWrapper.LowLevel
{
    public abstract class CameraImageListener
    {

        protected Camera.ImageUpdateHandler ImageUpdateHandler;

        public CameraImageListener()
        {
            ImageUpdateHandler = new Camera.ImageUpdateHandler(UpdateFrame);
        }

        /// <summary>
        /// Register and unregister for image callbacks
        /// </summary> 
        public virtual void RegisterForImages(ImagerBase c)
        {
            Logger.WriteLine("");
            if (c != null)
            {
                c.OnImageUpdated -= ImageUpdateHandler;
                c.OnImageUpdated += ImageUpdateHandler;
            }
        }
        public virtual void UnregisterForImages(ImagerBase c)
        {
            Logger.WriteLine("");
            if (c != null)
                c.OnImageUpdated -= ImageUpdateHandler;
        }

        /// <summary>
        ///  Processes a new frame from the camera and decides if event should be triggered
        /// </summary>
        public abstract void UpdateFrame(object sender, EventArgs e);

    }
}
