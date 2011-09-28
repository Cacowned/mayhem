﻿/*
 * PresenceDetectorComponent.cs
 * 
 * Low level object interfacing with the OpenCVDLL motion detector. 
 * 
 * (c) 2011, Microsoft Applied Sciences Group
 * 
 * Author: Sven Kratz
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using MayhemCore;
using Point = System.Drawing.Point;


namespace MayhemOpenCVWrapper.LowLevel
{
    /* <summary>
     * Wrapper for the C++ Motion Detector
     *  TODO: Make the detector classes more generic
     * <summary>
     * */
    public class MotionDetectorComponent : IVisionEventComponent
    {
        private bool VERBOSE_DEBUG = false;
        private OpenCVDLL.MotionDetector m;

        public delegate void DetectionHandler(object sender, List<Point> points);
        public event DetectionHandler OnMotionUpdate;

        //private Camera.ImageUpdateHandler imageUpdateHandler; 

        public Rect motionBoundaryRect = new Rect();

        private int frameCount = 0;

        private int[] contourPoints = new int[1200];
        private List<Point> points = new List<Point>();

        public MotionDetectorComponent(int width, int height)
        {
            m = new OpenCVDLL.MotionDetector(width, height);
        }

        ~MotionDetectorComponent()
        {
            Logger.WriteLine("dtor");
            m.Dispose();
        }

        public override void update_frame(object sender, EventArgs e)
        {
            Camera camera = sender as Camera;

            int numPoints = 0;

            for (int i = 0; i < contourPoints.Length; i++)
            {
                contourPoints[i] = 0; 
            }

            lock (camera.thread_locker)
            {
                unsafe
                {
                    fixed (byte* ptr = camera.imageBuffer)
                    {
                        fixed (int* buf = contourPoints)
                        {
                            m.ProcessFrame(ptr, buf, &numPoints);

                            //Marshal.Copy((IntPtr)buf, contourPoints, 0, numPoints);
                        }
                    }
                }
            }

            Logger.WriteLineIf(VERBOSE_DEBUG, "Got " + numPoints + " contourpoints");

            if (numPoints == 0) return;

            int cpIdx = 0;
          
            points.Clear();
            

            for (cpIdx = 0; cpIdx < numPoints; )
            {
                Point p1 = new Point(contourPoints[cpIdx++], contourPoints[cpIdx++]);
                Point p2 = new Point(contourPoints[cpIdx++], contourPoints[cpIdx++]);
                Logger.WriteLineIf(VERBOSE_DEBUG, "Point 1: " + p1 + " Point 2: " + p2);
                
                points.Add(p1);
                points.Add(p2);
            }

            /*
            if (OnMotionUpdate != null && points.Count() > 0 && frameCount >40)
                OnMotionUpdate(this, points); */

            // intelligently decide where the motion is

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

            Rect boundingRect = new Rect(pMin.X, pMin.Y, pMax.X - pMin.X, pMax.Y - pMin.Y);

            // create a copy of the intersection

            if ((motionBoundaryRect.Width == 0 && motionBoundaryRect.Height == 0) || (motionBoundaryRect.Width == 320 && motionBoundaryRect.Height == 240))
            {
                if (OnMotionUpdate != null && points.Count() > 0 && frameCount > 40)
                    OnMotionUpdate(this, points);
            }
            else if (boundingRect.IntersectsWith(motionBoundaryRect))
            {
                Rect intersection = new Rect(boundingRect.X, boundingRect.Y, boundingRect.Width, boundingRect.Height);
                intersection.Intersect(motionBoundaryRect);

                // compare the respective areas of motionBoundaryRect and the intersection
                // if the intersection area is > 1/3 then count this as a detected motion

                double motionBoundaryRectArea = motionBoundaryRect.Height * motionBoundaryRect.Width;
                double intersectionArea = intersection.Height * intersection.Width;

                if (intersectionArea / motionBoundaryRectArea > 0.3)
                {
                    // intersection area large enough --> send the update
                    if (OnMotionUpdate != null && points.Count() > 0 && frameCount > 40)
                        OnMotionUpdate(this, points);
                }
            }
            frameCount++;
        }

        /*
         * <summary>
         * Sets the motion boundary rectangle, called by the associated trigger object
         * </summary>
         */

        public void SetMotionBoundaryRect(Rect r)
        {
            if (!r.IsEmpty || r.Width != 0 || r.Height != 0)
            {
                motionBoundaryRect = r;
            }
            else
            {
                // default to an "empty" rectangle
                motionBoundaryRect = new Rect(0, 0, 0, 0);
            }
        }
    }
}
