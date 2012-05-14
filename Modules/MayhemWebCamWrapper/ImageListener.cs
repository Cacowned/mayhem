using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MayhemWebCamWrapper
{
    /// <summary>
    /// Classes depending on camera image updates will need to derivce from cameraImageListener.
    /// </summary>
    public class ImageListener : ImageListenerBase
    {   
        /// <summary>
        /// Protected so that only subclasses can use the constructor
        /// CameraImageListeners should not be used in non-subclasses
        /// </summary>
        protected ImageListener()
        {
            SubscribedImagers = new List<ImagerBase>();
            HasImageSource = false;
            ImageUpdateHandler = UpdateFrame;
        } 
    }
}
