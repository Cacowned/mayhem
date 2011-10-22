using System;
using MayhemCore;

namespace MayhemOpenCVWrapper.LowLevel
{
    /// <summary>
    /// Classes depending on camera image updates will need to derivce from cameraImagelistsner.
    /// </summary>
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
        /// <param name="c">ImagerBase to register</param>
        public virtual void RegisterForImages(ImagerBase c)
        {
            Logger.WriteLine("");
            if (c != null)
            {
                c.OnImageUpdated -= ImageUpdateHandler;
                c.OnImageUpdated += ImageUpdateHandler;
            }
        }

        /// <summary>
        /// Deregister from image callbacks
        /// </summary>
        /// <param name="c">ImagerBase to unregister</param>
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
