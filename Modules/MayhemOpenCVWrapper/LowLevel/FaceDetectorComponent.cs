/*
 *  FaceDetectirComponent.cs
 * 
 *  Face detector low-level wrapper. 
 *  Used by the Mayhem face detector module. 
 *  
 *  (c) 2011, Microsoft Applied Sciences Group
 *  
 *  Author: Sven Kratz
 *  
 */

using System;
using System.Collections.Generic;
using System.Windows;
using MayhemCore;
using OpenCVDLL;
using Point = System.Drawing.Point;

namespace MayhemOpenCVWrapper.LowLevel
{
    public class FaceDetectorComponent : CameraImageListener
    {
        private FaceDetector fd; 
        public delegate void DetectionHandler(object sender, List<Point> points);
        public event DetectionHandler OnFaceDetected;
        
        private int frameCount = 0;
        private const bool VerboseDebug = true;
        public Rect DetectionBoundary = new Rect(0, 0, 0, 0);

        public FaceDetectorComponent(ImagerBase c)
        {
            int width = c.Settings.ResX;
            int height = c.Settings.ResY;
            Logger.WriteLine("Face Detector: w {0} h {1}", width, height);
            fd = new FaceDetector(width, height);
        }

        ~FaceDetectorComponent()
        {
            Logger.WriteLine("dtor");
            fd.Dispose();
        }
       
        /// <summary>
        ///  Processes a new frame from the camera and decides if event should be triggered
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public override void UpdateFrame(object sender, EventArgs e)
        {
            Logger.WriteLine("frame nr "+frameCount);
            frameCount++;
            Camera camera = sender as Camera;
            int[] faceCoords = new int[1024];
            int numFacesCoords = 0;

            lock (camera.ThreadLocker)
            {
                unsafe
                {
                    fixed (byte* ptr = camera.ImageBuffer)
                    {
                        fixed (int* buf = faceCoords)
                        {
                            fd.ProcessFrame(ptr, buf, &numFacesCoords);
                        }
                    }
                }
            }

            Logger.WriteLineIf(VerboseDebug, ">>>>> Got " + numFacesCoords+ " face coords ");

            // no need to do further work if no faces have been detected
            // update listeners with an empty list to inform them that nothing has been detected
            if (numFacesCoords == 0)
            {
                if (OnFaceDetected != null)
                    OnFaceDetected(this, new List<Point>());
                return;
            }
                
            int cpIdx = 0;
            List<Point> points = new List<Point>();

            // copy the coordinates to a list
            for (cpIdx = 0; cpIdx < numFacesCoords; )
            {
                Point p1 = new Point(faceCoords[cpIdx++], faceCoords[cpIdx++]);
                Point p2 = new Point(faceCoords[cpIdx++], faceCoords[cpIdx++]);
                Logger.WriteLineIf(VerboseDebug, "Point 1: " + p1 + " Point 2: " + p2);

                points.Add(p1);
                points.Add(p2);
            }

            if (OnFaceDetected != null && /* points.Count() > 0 &&*/ frameCount > 20)
            {
                // fire immediately on empty bounding rect
                if (DetectionBoundary.Width == 0 && DetectionBoundary.Height == 0)
                {
                    if (OnFaceDetected != null)
                         OnFaceDetected(this, points);
                }
                else
                {
                    // decide whether the points are within the boundary

                    Point pMax = new Point(int.MinValue, int.MinValue);
                    Point pMin = new Point(int.MaxValue, int.MaxValue);

                    foreach (Point p in points)
                    {
                        if (p.X == 0 && p.Y == 0)
                            continue;
                        pMin.X = Math.Min(pMin.X, p.X);
                        pMax.X = Math.Max(pMax.X, p.X);
                        pMin.Y = Math.Min(pMin.Y, p.Y);
                        pMax.Y = Math.Max(pMax.Y, p.Y);
                    }

                    Rect dataBounds = new Rect(pMin.X, pMin.Y, pMax.X - pMin.X, pMax.Y - pMin.Y);

                    if (dataBounds.IntersectsWith(DetectionBoundary))
                    {
                        if (OnFaceDetected != null)
                            OnFaceDetected(this, points);
                    }
                }             
            }
        }

        /*
        * <summary>
        * Sets the motion boundary rectangle, called by the associated trigger object
        * </summary>
        */

        public void SetDetectoinBoundary(Rect r)
        {
            if (!r.IsEmpty || r.Width != 0 || r.Height != 0)
            {
                DetectionBoundary = r;
            }
            else
            {
                // default to an "empty" rectangle
                DetectionBoundary = new Rect(0, 0, 0, 0);
            }
        }


    }

       
}
