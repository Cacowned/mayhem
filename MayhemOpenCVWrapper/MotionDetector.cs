using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows;

using Point = System.Drawing.Point;


namespace MayhemOpenCVWrapper
{
    public  class MotionDetector
    {


        private bool VERBOSE_DEBUG = false;
        private OpenCVDLL.MotionDetector m ;

        public delegate void MotionUpdateHandler(object sender, List<Point> points);
        public event MotionUpdateHandler OnMotionUpdate;

        public Rect motionBoundaryRect = new Rect();


        private int frameCount = 0;


        public MotionDetector(int width, int height)
        {
           m = new OpenCVDLL.MotionDetector(width, height);
        }

        public void RegisterForImages(Camera c)
        {
            c.OnImageUpdated += new Camera.ImageUpdateHandler(update_frame);
        }

        public void update_frame(object sender, EventArgs e)
        {
           // throw new NotImplementedException();

            Camera camera = sender as Camera;
            
            int numPoints = 0;

            int[] contourPoints = new int[1200];

            lock (camera.thread_locker)
            {

                unsafe
                {
                    fixed (byte* ptr = camera.imageBuffer)
                    {
                        fixed (int* buf = contourPoints)
                        {

                            m.ProcessFrame(ptr, buf, &numPoints);

                            Marshal.Copy((IntPtr)buf, contourPoints, 0, numPoints);
 
                        }
                    }
                 }

                Debug.WriteLineIf(VERBOSE_DEBUG,"Got " + numPoints + " contourpoints");

                if (numPoints == 0) return;

                int cpIdx = 0;

                List<Point> points = new List<Point>();


                
                for (cpIdx = 0; cpIdx < numPoints; )
                {
                    Point p1 = new Point(contourPoints[cpIdx++], contourPoints[cpIdx++]);
                    Point p2 = new Point(contourPoints[cpIdx++], contourPoints[cpIdx++]);
                    Debug.WriteLineIf(VERBOSE_DEBUG,"Point 1: " + p1 + " Point 2: " + p2);

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

                if (motionBoundaryRect.Width == 0 && motionBoundaryRect.Height == 0)
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

                    if (intersectionArea / motionBoundaryRectArea > 1 / 2)
                    {
                        // intersection area large enough --> send the update
                           if (OnMotionUpdate != null && points.Count() > 0 && frameCount >40)
                               OnMotionUpdate(this, points); 
                    }
                }
                frameCount++;
            }
        }

        /*
         * <summary>
         * Sets the motion boundary rectangle, called by the assiciated trigger object
         * </summary>
         */

        public void SetMotionBoundaryRect(Rect r)
        {
            if (!r.IsEmpty)
            {
                motionBoundaryRect = r;
            }
            else
            {
                motionBoundaryRect = new Rect(0, 0, 320, 240);
            }
        }
    }
}
