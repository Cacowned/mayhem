using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Interop;

namespace MayhemWebCamWrapper
{
    /// <summary>
    /// Errors thrown by various imagers
    /// </summary>
    [Serializable]
    public class ImageException : Exception
    {
        public string ErrorMessage
        {
            get
            {
                return base.Message.ToString();
            }
        }

        public ImageException(string errorMessage) : base(errorMessage) { }
        public ImageException(string errorMessage, Exception innerEx) : base(errorMessage, innerEx) { } 
    }

    /// <summary>
    /// Abstract base class for imaging devices, i.e. cameras or video capture cards.
    /// </summary>
    public abstract class ImagerBase
    {
        ///<summary>
        ///Get and set dimensions of the imager
        ///</summary>
        ///
        public abstract int Width
        {
            get;
            set;
        }
        public abstract int Height
        {
            get;
            set;
        }
        
        /// <summary>
        /// Image update handler. Modules that want regular camera image updates should attach to this handler. 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public delegate void ImageUpdateHandler(object sender, EventArgs e);

        /// <summary>
        /// Image update event 
        /// </summary>
        public abstract event ImageUpdateHandler OnImageUpdated;

        /// <summary>
        /// Starts the camera and delivery of image update events
        /// </summary>
        public abstract void Start();

        /// <summary>
        /// Attempts to stop the camera. If listeners are still attached to OnImageUpdate, the camera will not shut down. 
        /// </summary>
        /// <returns>true if the camera has been successfuly shut down, else false</returns>
        public abstract void Stop();


        /// <summary>
        /// List of subscribers
        /// </summary>
    
        public List<ImageListenerBase> Subscribers;
    }

}
