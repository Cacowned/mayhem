/*
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
    /// <summary>
    /// Wrapper for the C++ Motion Detector 
    /// </summary>
    public class MotionDetectorComponent : CameraImageListener
    {
        public delegate void DetectionHandler(object sender, List<Point> points);
        public event DetectionHandler OnMotionUpdate;
        public Rect MotionBoundaryRect = new Rect();

        private bool VERBOSE_DEBUG = false;
        private OpenCVDLL.MotionDetector m;
        private int frameCount = 0;
        private int[] contourPoints = new int[1200];
        private List<Point> points = new List<Point>();

        public MotionDetectorComponent(ImagerBase c)
        {
            int width = c.Settings.ResX;
            int height = c.Settings.ResY;
            m = new OpenCVDLL.MotionDetector(width, height);
        }

        ~MotionDetectorComponent()
        {
            Logger.WriteLine("dtor");
            m.Dispose();
        }

        public override void UpdateFrame(object sender, EventArgs e)
        {
            Camera camera = sender as Camera;

            int numPoints = 0;

            for (int i = 0; i < contourPoints.Length; i++)
            {
                contourPoints[i] = 0; 
            }

            lock (camera.ThreadLocker)
            {
                unsafe
                {
                    fixed (byte* ptr = camera.ImageBuffer)
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

            // decide the location of the motion 

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

            if ((MotionBoundaryRect.Width == 0 && MotionBoundaryRect.Height == 0) || (MotionBoundaryRect.Width == 320 && MotionBoundaryRect.Height == 240))
            {
                if (OnMotionUpdate != null && points.Count() > 0 && frameCount > 40)
                    OnMotionUpdate(this, points);
            }
            else if (boundingRect.IntersectsWith(MotionBoundaryRect))
            {
                Rect intersection = new Rect(boundingRect.X, boundingRect.Y, boundingRect.Width, boundingRect.Height);
                intersection.Intersect(MotionBoundaryRect);

                // compare the respective areas of motionBoundaryRect and the intersection
                // if the intersection area is > 1/3 then count this as a detected motion

                double motionBoundaryRectArea = MotionBoundaryRect.Height * MotionBoundaryRect.Width;
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
    
        /// <summary>
        /// Sets the motion boundary rectangle, called by the associated trigger object
        /// </summary>
        public void SetMotionBoundaryRect(Rect r)
        {
            if (!r.IsEmpty || r.Width != 0 || r.Height != 0)
            {
                MotionBoundaryRect = r;
            }
            else
            {
                // default to an "empty" rectangle
                MotionBoundaryRect = new Rect(0, 0, 0, 0);
            }
        }
    }
}
