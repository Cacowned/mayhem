using System;

namespace MayhemOpenCVWrapper
{
    /// <summary>
    /// A dummy class acting as a camera listener. 
    /// Fake Camera Listener used by Picture and Vidoe to keep subscribed to camera updates
    /// </summary>
    public class CameraImageListenerBase
    {
        protected ImagerBase.ImageUpdateHandler imageUpdateHandler;

        /// <summary>
        /// The frame update function
        /// Subclasses can override this method to implement their specific functionality when
        /// receiving a frame update
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public virtual void UpdateFrame(object sender, EventArgs e) 
        { }

        /// <summary>
        /// Register and unregister for image callbacks
        /// </summary> 
        /// <param name="c">ImagerBase to register</param>
        public void RegisterForImages(ImagerBase c)
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
        public void UnregisterForImages(ImagerBase c)
        {
            if (c != null)
                c.OnImageUpdated -= imageUpdateHandler;
        } 

    }
}
