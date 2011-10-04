/*
 * ObjectDetectorComponent.cs
 * 
 * Low level object interfacing with the OpenCVDLL object detector component. 
 * 
 * (c) 2011, Microsoft Applied Sciences Group
 * 
 * Author: Sven Kratz
 * 
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using MayhemCore;
using OpenCVDLL;
using Point = System.Drawing.Point;

namespace MayhemOpenCVWrapper.LowLevel
{
    public class ObjectDetectorComponent : CameraImageListener
    {
        public delegate void DetectionHandler(object sender, List<Point> points);
        public event DetectionHandler OnObjectDetected;

        //private Camera.ImageUpdateHandler imageUpdateHandler;
        private int frameCount = 0;
        private Bitmap templateImg = null; 
        private SURFObjectDetector od;

        // prerequisite for this module to work is a set image template
        public bool TemplateIsSet = false;
        
        // detection threshold --> TODO: make changeable by GUI
        public int DetectThresh = 4;

        public Point[] LastCornerPoints
        {
            get; 
            private set ;
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
            od = new OpenCVDLL.SURFObjectDetector(width, height); 
        }

       /** <summary>
        *  Method to set the template image for the object detector
        * </summary>
        */
        public void SetTemplate(Bitmap templateImage)
        {
           
            templateImg = templateImage;

            int w = templateImage.Width;
            int h = templateImage.Height;

            if (templateImage.PixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb)
            {
                int bufSize = w * h * 3;
                byte[] imgBuf = new byte[bufSize];
                Rectangle rect  = new Rectangle(0,0, w,h);
                System.Drawing.Imaging.BitmapData bmpData =
                    templateImage.LockBits( rect, 
                                            System.Drawing.Imaging.ImageLockMode.ReadOnly, 
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
                        od.AddTemplate(w, h, ptr);
                    }
                }
                TemplateIsSet = true;
            }
            else
            {
                Logger.WriteLine("Pixel Format not supported yet!");
                TemplateIsSet = false;
                throw(new NotImplementedException());
            }
        }

        public override void UpdateFrame(object sender, EventArgs e)
        {
            Logger.WriteLine("frame nr " + frameCount++);
            Camera camera = sender as Camera;

            if (TemplateIsSet)
            {
                // transmit frame
                lock (camera.ThreadLocker)
                {
                    unsafe
                    {
                        fixed (byte* ptr = camera.ImageBuffer)
                        {
                            od.ProcessFrame(ptr);
                        }
                    }
                }

                // get feature correspondences

                // template keypoints
                OpenCVDLL.SURFKeyPoint[] tKeyPoints = new OpenCVDLL.SURFKeyPoint[1024];
                int nTKeyPts = 0; 
                // image keypoints
                OpenCVDLL.SURFKeyPoint[] iKeyPoints = new OpenCVDLL.SURFKeyPoint[1024];
                int nIKeyPts = 0; 
                // pair indices
                int[] matchPairIndices = new int[2048];
                int nMatchingPairs = 0;

                unsafe
                {
                    fixed (OpenCVDLL.SURFKeyPoint* tkPtr = tKeyPoints)
                    {
                        fixed (OpenCVDLL.SURFKeyPoint* ikPtr = iKeyPoints)
                        {
                            fixed (int* pairPtr = matchPairIndices)
                            {
                                od.getKeypointsAndMatches(tkPtr, &nTKeyPts, ikPtr, &nIKeyPts, pairPtr, &nMatchingPairs);
                            }
                        }
                    }
                }

                Logger.WriteLine("==== Results of SURF descriptor calculation ====");
                Logger.WriteLine("TemplateKeyPoints: " + nTKeyPts + " ImageKeyPoints " + nIKeyPts + " nMatches " + nMatchingPairs);
                Logger.Write("Matching Indices: ");

                // template key points
                List<Point> tempKeyPoints = new List<Point>();
                for (int i = 0; i < nTKeyPts; i++)
                {
                    tempKeyPoints.Add(new Point((int) tKeyPoints[i].x, (int) tKeyPoints[i].y));
                }
                this.TemplateKeyPoints = tempKeyPoints;

                // image key points
                List<Point> imageKeyPoints = new List<Point>();
                for (int i = 0; i < nIKeyPts; i++)
                {
                    imageKeyPoints.Add(new Point((int)iKeyPoints[i].x, (int)iKeyPoints[i].y));
                }
                this.LastImageKeyPoints = imageKeyPoints;

                // point correspondences
                for (int i = 0; i < nMatchingPairs; i+=2)
                {
                    Logger.Write(matchPairIndices[i] + "," + matchPairIndices[i + 1] + " ");
                }

                // calculate list of matching points in input image
                List<Point> imageMatchingPoints = new List<Point>();

                // even are points in template, odd are points in camera image

                for (int i = 0; i < nMatchingPairs; i+=2)
                {
                    int tIdx = matchPairIndices[i];
                    int iIdx = matchPairIndices[i+1];

                    OpenCVDLL.SURFKeyPoint tPpt = tKeyPoints[tIdx];
                    OpenCVDLL.SURFKeyPoint iPpt = iKeyPoints[iIdx];

                    Point tPoint = new Point((int)tPpt.x, (int)tPpt.y);
                    Point iPoint = new Point((int)iPpt.x, (int)iPpt.y);

                    imageMatchingPoints.Add(tPoint);
                    imageMatchingPoints.Add(iPoint);
                }

                LastImageMatchingPoints = imageMatchingPoints;

                // planar object correspondence
                int[] cornerPoints = new int[8];
                int res = 0;
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
                if (OnObjectDetected != null && imageMatchingPoints.Count>= DetectThresh && frameCount > 20)
                {
                    OnObjectDetected(this, imageMatchingPoints);
                }
                // --------------------------------------------
            }
        }
    }
}
