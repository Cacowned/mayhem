using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using MayhemCore;
using MayhemOpenCVWrapper;
using OpenCVDLL;
using Point = System.Drawing.Point;

namespace VisionModules.Components
{
    /// <summary>
    /// Face detector component of the face detector event. Can also be used as a standalone face detector. 
    /// </summary>
    public class FaceDetectorComponent : CameraImageListener, IDisposable
    {
        public delegate void DetectionHandler(object sender, DetectionEventArgs e);

        public event DetectionHandler OnFaceDetected;

        public Rect DetectionBoundary
        {
            get;
            set;
        }

        private readonly FaceDetector faceDetector;
        private int frameCount;
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
            DetectionBoundary = new Rect(0, 0, 0, 0);
            faceDetector = new FaceDetector(width, height);
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
            Logger.WriteLine("frame nr " + frameCount);
            frameCount++;
            Camera camera = sender as Camera;
            int[] faceCoords = new int[1024];
            int numFacesCoords = 0;

            Bitmap cameraImage = camera.ImageAsBitmap();

            BitmapData bd = cameraImage.LockBits(new Rectangle(0, 0, cameraImage.Size.Width, cameraImage.Size.Height), ImageLockMode.ReadOnly, cameraImage.PixelFormat);

            IntPtr imgPointer = bd.Scan0;

            // transmit frame
            unsafe
            {
                fixed (int* buf = faceCoords)
                {
                    faceDetector.ProcessFrame((byte*)imgPointer, buf, &numFacesCoords);
                }
            }

            cameraImage.UnlockBits(bd);
            cameraImage.Dispose();

            Logger.WriteLineIf(VerboseDebug, ">>>>> Got " + numFacesCoords + " face coords ");

            // no need to do further work if no faces have been detected
            // update listeners with an empty list to inform them that nothing has been detected
            if (numFacesCoords == 0)
            {
                if (OnFaceDetected != null)
                    OnFaceDetected(this, new DetectionEventArgs(new List<Point>()));

                return;
            }

            List<Point> points = new List<Point>();

            // copy the coordinates to a list
            for (int indexCoord = 0; indexCoord < numFacesCoords;)
            {
                Point p1 = new Point(faceCoords[indexCoord++], faceCoords[indexCoord++]);
                Point p2 = new Point(faceCoords[indexCoord++], faceCoords[indexCoord++]);
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
                    Point maxPoint = new Point(int.MinValue, int.MinValue);
                    Point minPoint = new Point(int.MaxValue, int.MaxValue);

                    foreach (Point p in points)
                    {
                        if (p.X == 0 && p.Y == 0)
                            continue;
                        minPoint.X = Math.Min(minPoint.X, p.X);
                        maxPoint.X = Math.Max(maxPoint.X, p.X);
                        minPoint.Y = Math.Min(minPoint.Y, p.Y);
                        maxPoint.Y = Math.Max(maxPoint.Y, p.Y);
                    }

                    Rect dataBounds = new Rect(minPoint.X, minPoint.Y, maxPoint.X - minPoint.X, maxPoint.Y - minPoint.Y);

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
                faceDetector.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
