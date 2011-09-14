/*
 * ImagerBase.cs
 * 
 * Abstract base class for imaging devices, i.e. cameras of video capture cards. 
 * 
 * (c) 2011, Microsoft Applied Sciences Group
 * 
 * Author: Sven Kratz
 * 
 */ 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace MayhemOpenCVWrapper
{
   
    public abstract class ImagerBase
    {
          public bool running; 
          public delegate void ImageUpdateHandler(object sender, EventArgs e);
          public abstract event ImageUpdateHandler OnImageUpdated;
          public abstract Bitmap ImageAsBitmap();
          public abstract void StartFrameGrabbing();
          public abstract bool TryStopFrameGrabbing();
    }
}
