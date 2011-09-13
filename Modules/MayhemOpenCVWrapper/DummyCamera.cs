/*
 * 
 * DummyCamera.cs
 * 
 * A dummy camera implementation. Used when Mayhem has no imaging device attached. 
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
using System.Diagnostics;

namespace MayhemOpenCVWrapper
{
    public class DummyCamera : ImagerBase
    {
        public override event ImageUpdateHandler OnImageUpdated;

        public const string TAG = "[DummyCamera] :";
        public  override Bitmap ImageAsBitmap()
        {
            Debug.WriteLine(TAG + "returning dummy image");
            // todo: add Mayhem's logo or something 
            return new Bitmap(320, 240);
        }

        public override  void StartFrameGrabbing()
        {
            Debug.WriteLine(TAG + "StartFrameGrabbing");
            // no-op
        }

        public override bool TryStopFrameGrabbing()
        {
            Debug.WriteLine(TAG + "TryStopFrameGrabbing");
            return true; 
        }
    }
}
