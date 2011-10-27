using System;

namespace MayhemOpenCVWrapper
{
    /// <summary>
    /// Classes depending on camera image updates will need to derivce from cameraImageListener.
    /// </summary>
    public class CameraImageListener : CameraImageListenerBase
    {   
        /// <summary>
        /// Protected so that only subclasses can use the constructor
        /// CameraImageListeners should not be used in non-subclasses
        /// </summary>
        protected CameraImageListener()
        {
            imageUpdateHandler = UpdateFrame;
        } 
    }
}
