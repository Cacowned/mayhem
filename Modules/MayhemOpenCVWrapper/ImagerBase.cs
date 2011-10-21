/*
 * ImagerBase.cs
 * 
 * Abstract base class for imaging devices, i.e. cameras or video capture cards. 
 * 
 * (c) 2011, Microsoft Applied Sciences Group
 * 
 * Author: Sven Kratz
 * 
 */ 
using System;
using System.Drawing;

namespace MayhemOpenCVWrapper
{
    public abstract class ImagerBase
    {
        /// <summary>
        /// Camera information
        /// </summary>
        public abstract CameraInfo Info { get; protected set; }

        /// <summary>
        /// Camera settings
        /// </summary>
        public abstract CameraSettings Settings { get; protected set; }

        /// <summary>
        /// The camera's running state
        /// </summary>
        public abstract bool Running { get; protected set; }

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
        /// Retrieve the camera image as a bitmap 
        /// </summary>
        /// <returns>Bitmap containing the camera image</returns>
        public abstract Bitmap ImageAsBitmap();

        /// <summary>
        /// Starts the camera and delivery of image update events
        /// </summary>
        public abstract void StartFrameGrabbing();

        /// <summary>
        /// Attempts to stop the camera. If listeners are still attached to OnImageUpdate, the camera will not shut down. 
        /// </summary>
        /// <returns>true if the camera has been successfuly shut down, else false</returns>
        public abstract bool TryStopFrameGrabbing();
    }
}
