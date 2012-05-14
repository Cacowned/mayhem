using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using MayhemCore;
using OpenCVDLL;
using Point = System.Drawing.Point;
using MayhemOpenCVWrapper;

namespace VisionModules.Components
{
    /// <summary>
    /// Low level object interfacing with the OpenCVDLL object detector component. 
    /// </summary>
    public class ObjectDetectorComponent : CameraImageListener
    {
        public delegate void DetectionHandler(object sender, DetectionEventArgs e);

        public event DetectionHandler OnObjectDetected;

        private int frameCount;
        private readonly SURFObjectDetector od;

        // prerequisite for this module to work is a set image template
        public bool TemplateIsSet
        {
            get;
            set;
        }

        // detection threshold
        private const int DetectionThreshold = 4;

        public Point[] LastCornerPoints
        {
            get;
            private set;
        }

        public List<Point> TemplateKeyPoints
        {
            get;
            private set;
        }

        public List<Point> LastImageKeyPoints
        {
            get;
            private set;
        }

        public List<Point> LastImageMatchingPoints
        {
            get;
            private set;
        }

        public ObjectDetectorComponent(int width, int height)
        {
            od = new SURFObjectDetector(width, height);
        }

        /// <summary>
        /// Method to set the template image for the object detector
        /// </summary>
        /// <param name="templateImage">The image to use as the template</param>
        public void SetTemplate(Bitmap templateImage)
        {
            int width = templateImage.Width;
            int height = templateImage.Height;

            if (templateImage.PixelFormat == PixelFormat.Format24bppRgb)
            {
                int bufSize = width * height * 3;
                byte[] imgBuf = new byte[bufSize];
                Rectangle rect = new Rectangle(0, 0, width, height);
                BitmapData bmpData =
                    templateImage.LockBits(rect,
                                            ImageLockMode.ReadOnly,
                                            templateImage.PixelFormat);

                IntPtr dataPtr = bmpData.Scan0;

                // copy image data to buffer
                Marshal.Copy(dataPtr, imgBuf, 0, bufSize);
                templateImage.UnlockBits(bmpData);

                // call c++ code
                unsafe
                {
                    fixed (byte* ptr = imgBuf)
                    {
                        od.AddTemplate(width, height, ptr);
                    }
                }

                TemplateIsSet = true;
            }
            else
            {
                Logger.WriteLine("Pixel Format not supported yet!");
                TemplateIsSet = false;
                throw new NotImplementedException();
            }
        }

        public override void UpdateFrame(object sender, EventArgs e)
        {
            Logger.WriteLine("frame nr " + frameCount++);
            Camera camera = sender as Camera;

            if (TemplateIsSet)
            {
                Bitmap cameraImage = camera.ImageAsBitmap();
                BitmapData bd = cameraImage.LockBits(new Rectangle(0, 0, cameraImage.Size.Width, cameraImage.Size.Height), ImageLockMode.ReadOnly, cameraImage.PixelFormat);
                IntPtr imgPointer = bd.Scan0;

                // transmit frame
                unsafe
                {
                    od.ProcessFrame((byte*)imgPointer);
                }

                cameraImage.UnlockBits(bd);
                cameraImage.Dispose();

                // get feature correspondences

                // template keypoints
                SURFKeyPoint[] templateKeyPoints = new SURFKeyPoint[1024];
                int numTemplateKeyPts = 0;

                // image keypoints
                SURFKeyPoint[] imgKeyPoints = new SURFKeyPoint[1024];
                int numImgKeyPts = 0;

                // pair indices
                int[] matchPairIndices = new int[2048];
                int numMatchingPairs = 0;

                unsafe
                {
                    fixed (SURFKeyPoint* templeteKeyPointsPtr = templateKeyPoints)
                    {
                        fixed (SURFKeyPoint* imageKeyPointsPtr = imgKeyPoints)
                        {
                            fixed (int* pairPtr = matchPairIndices)
                            {
                                od.getKeypointsAndMatches(templeteKeyPointsPtr, &numTemplateKeyPts, imageKeyPointsPtr, &numImgKeyPts, pairPtr, &numMatchingPairs);
                            }
                        }
                    }
                }

                Logger.WriteLine("==== Results of SURF descriptor calculation ====");
                Logger.WriteLine("TemplateKeyPoints: " + numTemplateKeyPts + " ImageKeyPoints " + numImgKeyPts + " nMatches " + numMatchingPairs);
                Logger.Write("Matching Indices: ");

                // template key points
                List<Point> tempKeyPoints = new List<Point>();
                for (int i = 0; i < numTemplateKeyPts; i++)
                {
                    tempKeyPoints.Add(new Point((int)templateKeyPoints[i].x, (int)templateKeyPoints[i].y));
                }

                TemplateKeyPoints = tempKeyPoints;

                // image key points
                List<Point> imageKeyPoints = new List<Point>();
                for (int i = 0; i < numImgKeyPts; i++)
                {
                    imageKeyPoints.Add(new Point((int)imgKeyPoints[i].x, (int)imgKeyPoints[i].y));
                }

                LastImageKeyPoints = imageKeyPoints;

                // point correspondences
                for (int i = 0; i < numMatchingPairs; i += 2)
                {
                    Logger.Write(matchPairIndices[i] + "," + matchPairIndices[i + 1] + " ");
                }

                // calculate list of matching points in input image
                List<Point> imageMatchingPoints = new List<Point>();

                // even are points in template, odd are points in camera image
                for (int i = 0; i < numMatchingPairs; i += 2)
                {
                    int templateIdx = matchPairIndices[i];
                    int imageIdx = matchPairIndices[i + 1];

                    SURFKeyPoint templatePpt = templateKeyPoints[templateIdx];
                    SURFKeyPoint imagePpt = imgKeyPoints[imageIdx];

                    Point templatePoint = new Point((int)templatePpt.x, (int)templatePpt.y);
                    Point imagePoint = new Point((int)imagePpt.x, (int)imagePpt.y);

                    imageMatchingPoints.Add(templatePoint);
                    imageMatchingPoints.Add(imagePoint);
                }

                LastImageMatchingPoints = imageMatchingPoints;

                // planar object correspondence
                int[] cornerPoints = new int[8];
                int res;

                unsafe
                {
                    fixed (int* cpts = cornerPoints)
                    {
                        res = od.findLastObjectCorners(cpts);
                    }
                }

                // assign the retrieved cornerpoints 
                if (res > 0)
                {
                    LastCornerPoints = new Point[4];
                    int j = 0;
                    for (int k = 0; k < 4; k++)
                    {
                        Point cp = new Point(cornerPoints[j++], cornerPoints[j++]);
                        LastCornerPoints[k] = cp;
                    }
                }
                else
                {
                    LastCornerPoints = null;
                }

                // TODO: --------------------- firing mechanism
                if (OnObjectDetected != null && imageMatchingPoints.Count >= DetectionThreshold && frameCount > 20)
                {
                    OnObjectDetected(this, new DetectionEventArgs(imageMatchingPoints));
                }
            }
        }
    }
}
