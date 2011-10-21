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
using System.Drawing;
using System.Drawing.Imaging;

namespace MayhemOpenCVWrapper.LowLevel
{

    /// <summary>
    /// Face detector component of the face detector event. Can also be used as a standalone face detector. 
    /// </summary>
    public class FaceDetectorComponent : CameraImageListener, IDisposable
    {     
        public delegate void DetectionHandler(object sender, DetectionEventArgs e);
        public event DetectionHandler OnFaceDetected;
        public Rect DetectionBoundary = new Rect(0, 0, 0, 0);

        private FaceDetector fd;
        private int frameCount = 0;
        private const bool VerboseDebug = true;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="c">ImagerBase that provides frame updates</param>
        public FaceDetectorComponent(ImagerBase c)
        {
            int width = c.Settings.ResX;
            int height = c.Settings.ResY;
            Logger.WriteLine("Face Detector: w {0} h {1}", width, height);
            fd = new FaceDetector(width, height);
        }

        ~FaceDetectorComponent()
        {
            Dispose(false);
            return;
        }
       
        /// <summary>
        ///  Processes a new frame from the camera and decides if event should be triggered
        /// </summary>
        public override void UpdateFrame(object sender, EventArgs e)
        {
            Logger.WriteLine("frame nr "+frameCount);
            frameCount++;
            Camera camera = sender as Camera;
            int[] faceCoords = new int[1024];
            int numFacesCoords = 0;

            Bitmap cameraImage = camera.ImageAsBitmap();

            BitmapData bd = cameraImage.LockBits(new Rectangle(0, 0, cameraImage.Size.Width, cameraImage.Size.Height), System.Drawing.Imaging.ImageLockMode.ReadOnly, cameraImage.PixelFormat);

            IntPtr imgPointer = bd.Scan0;

            // transmit frame
           
            unsafe
            {
                
                fixed (int* buf = faceCoords)
                {
                    fd.ProcessFrame((byte *) imgPointer, buf, &numFacesCoords);
                }
                
            }
            
            cameraImage.UnlockBits(bd);
            cameraImage.Dispose();

            Logger.WriteLineIf(VerboseDebug, ">>>>> Got " + numFacesCoords+ " face coords ");

            // no need to do further work if no faces have been detected
            // update listeners with an empty list to inform them that nothing has been detected
            if (numFacesCoords == 0)
            {
                if (OnFaceDetected != null)
                    OnFaceDetected(this, new DetectionEventArgs(new List<Point>()));
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
                         OnFaceDetected(this, new DetectionEventArgs(points));
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
                            OnFaceDetected(this, new DetectionEventArgs(points));
                    }
                }             
            }
        }

        
        
        /// <summary>
        /// Sets the motion boundary rectangle, called by the associated trigger object
        /// </summary>
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

        protected virtual void Dispose(bool dispose)
        {
            if (dispose)
            {

            }
            else
            {
                fd.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }

       
}
