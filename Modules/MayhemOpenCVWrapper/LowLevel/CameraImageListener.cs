using System;

namespace MayhemOpenCVWrapper.LowLevel
{
    /// <summary>
    /// Classes depending on camera image updates will need to derivce from cameraImagelistsner.
    /// </summary>
    public abstract class CameraImageListener
    {
        private readonly ImagerBase.ImageUpdateHandler imageUpdateHandler;

        protected CameraImageListener()
        {
            imageUpdateHandler = UpdateFrame;
        }

        /// <summary>
        /// Register and unregister for image callbacks
        /// </summary> 
        /// <param name="c">ImagerBase to register</param>
        public virtual void RegisterForImages(ImagerBase c)
        {
            if (c != null)
            {
                c.OnImageUpdated -= imageUpdateHandler;
                c.OnImageUpdated += imageUpdateHandler;
            }
        }

        /// <summary>
        /// Deregister from image callbacks
        /// </summary>
        /// <param name="c">ImagerBase to unregister</param>
        public virtual void UnregisterForImages(ImagerBase c)
        {
            if (c != null)
                c.OnImageUpdated -= imageUpdateHandler;
        }

        /// <summary>
        ///  Processes a new frame from the camera and decides if event should be triggered
        /// </summary>
        public abstract void UpdateFrame(object sender, EventArgs e);
    }
}
