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
using System.Linq;
using System.Text;
using Point = System.Drawing.Point;
using System.Runtime.InteropServices;
using System.Diagnostics;
using MayhemOpenCVWrapper;
using OpenCVDLL;
using System.Windows;
using MayhemCore;

namespace MayhemOpenCVWrapper.LowLevel
{
    public class FaceDetectorComponent : IVisionEventComponent
    {
        private FaceDetector fd; 
        public delegate void DetectionHandler(object sender, List<Point> points);
        public event DetectionHandler OnFaceDetected;
        
        private int frameCount = 0;
        private const bool VERBOSE_DEBUG = true;
        public Rect detectionBoundary = new Rect(0, 0, 0, 0);

        public FaceDetectorComponent()
        {
            fd = new FaceDetector(320, 240);
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
        public override void update_frame(object sender, EventArgs e)
        {
            Logger.WriteLine("frame nr "+frameCount);
            frameCount++;
            Camera camera = sender as Camera;
            int[] faceCoords = new int[1024];
            int numFacesCoords = 0;

            lock (camera.thread_locker)
            {
                unsafe
                {
                    fixed (byte* ptr = camera.imageBuffer)
                    {
                        fixed (int* buf = faceCoords)
                        {
                            fd.ProcessFrame(ptr, buf, &numFacesCoords);
                        }
                    }
                }
            }

            Logger.WriteLineIf(VERBOSE_DEBUG, ">>>>> Got " + numFacesCoords+ " face coords ");

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
                Logger.WriteLineIf(VERBOSE_DEBUG, "Point 1: " + p1 + " Point 2: " + p2);

                points.Add(p1);
                points.Add(p2);
            }

            if (OnFaceDetected != null && /* points.Count() > 0 &&*/ frameCount > 20)
            {
                // fire immediately on empty bounding rect
                if (detectionBoundary.Width == 0 && detectionBoundary.Height == 0)
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

                    if (dataBounds.IntersectsWith(detectionBoundary))
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
                detectionBoundary = r;
            }
            else
            {
                // default to an "empty" rectangle
                detectionBoundary = new Rect(0, 0, 0, 0);
            }
        }


    }

       
}
