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
          abstract public CameraInfo Info { get; protected set;  }
          abstract public CameraSettings Settings { get; protected set; }
          abstract public bool running{ get; protected set;}
          public delegate void ImageUpdateHandler(object sender, EventArgs e);
          public abstract event ImageUpdateHandler OnImageUpdated;
          public abstract Bitmap ImageAsBitmap();
          public abstract void StartFrameGrabbing();
          public abstract bool TryStopFrameGrabbing();
    }
}
