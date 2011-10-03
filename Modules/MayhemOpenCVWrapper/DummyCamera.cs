﻿/*
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

using System.Drawing;
using MayhemCore;

namespace MayhemOpenCVWrapper
{
    public class DummyCamera : ImagerBase
    {
        public override event ImageUpdateHandler OnImageUpdated;

        private  CameraInfo info = CameraInfo.DummyInfo();
        public override CameraInfo Info { get; protected set; }

        public override CameraSettings Settings
        {
            get;
            protected set;
        }

        public override bool running
        {
            get;
            protected set;
        }

        public  override Bitmap ImageAsBitmap()
        {
            Logger.WriteLine("returning dummy image");
            // todo: add Mayhem's logo or something 
            return new Bitmap(320, 240);
        }

        public override  void StartFrameGrabbing()
        {
            Logger.WriteLine("StartFrameGrabbing");
            // no-op
        }

        public override bool TryStopFrameGrabbing()
        {
            Logger.WriteLine("TryStopFrameGrabbing");
            return true; 
        }
    }
}
