using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Point = System.Drawing.Point;
using MayhemOpenCVWrapper;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Drawing;
using OpenCVDLL;

namespace MayhemOpenCVWrapper.LowLevel
{
    public class ObjectDetectorComponent : IVisionEventComponent
    {
        public static string TAG = "[ObjectDetector] : ";
        public delegate void DetectionHandler(object sender, List<Point> points);
        public event DetectionHandler OnObjectDetected;

        //private Camera.ImageUpdateHandler imageUpdateHandler;
        private int frameCount = 0;

        private Bitmap templateImg = null; 

        private SURFObjectDetector od;

        // prerequisite for this module to work is a set image template
        public bool templateIsSet = false;


        
        // detection threshold --> TODO: make changeable by GUI
        public int DETECT_THRESH = 4;

        private Point[] lastCornerPoints_ = null;
        public Point[] lastCornerPoints
        {
            get { return lastCornerPoints_; }
            set { }
        }

        private List<Point> templateKeyPoints_ = null;
        public List<Point> templateKeyPoints
        {
            get { return templateKeyPoints_; }
            set {}
        }

        private List<Point> lastImageKeyPoints_ = null;
        public List<Point> lastImageKeyPoints
        {
            get { return lastImageKeyPoints_; }
            set {}
        }

        private List<Point> lastImageMatchingPoints_ = null;
        public List<Point> lastImageMatchingPoints 
        {
            get { return lastImageMatchingPoints_;}
            set {}
        }

        public ObjectDetectorComponent(int width, int height)
        {
            od = new OpenCVDLL.SURFObjectDetector(width, height); 
        }

        

       /** <summary>
        *  Method to set the template image for the object detector
        * </summary>
        */
        public void set_template(Bitmap templateImage)
        {
           
            templateImg = templateImage;

            int w = templateImage.Width;
            int h = templateImage.Height;

            if (templateImage.PixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb)
            {
                int buf_size = w * h * 3;
                byte[] img_buf = new byte[buf_size];
                Rectangle rect  = new Rectangle(0,0, w,h);
                System.Drawing.Imaging.BitmapData bmpData =
                    templateImage.LockBits( rect, 
                                            System.Drawing.Imaging.ImageLockMode.ReadOnly, 
                                            templateImage.PixelFormat);

                IntPtr data_ptr = bmpData.Scan0;

                // copy image data to buffer
                Marshal.Copy(data_ptr, img_buf, 0, buf_size);
                templateImage.UnlockBits(bmpData);

                // call c++ code
                unsafe
                {
                    fixed (byte* ptr = img_buf)
                    {
                        od.AddTemplate(w, h, ptr);
                    }
                }
                templateIsSet = true;
            }
            else
            {
                Debug.WriteLine("Pixel Format not supported yet!");
                templateIsSet = false;
                throw(new NotImplementedException());
            }
        }

        public override void update_frame(object sender, EventArgs e)
        {
            Debug.WriteLine(TAG + "frame nr " + frameCount++);
            Camera camera = sender as Camera;

            if (templateIsSet)
            {

                // transmit frame
                lock (camera.thread_locker)
                {

                    unsafe
                    {
                        fixed (byte* ptr = camera.imageBuffer)
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
                    fixed (OpenCVDLL.SURFKeyPoint* tk_ptr = tKeyPoints)
                    {
                        fixed (OpenCVDLL.SURFKeyPoint* ik_ptr = iKeyPoints)
                        {
                            fixed (int* pair_ptr = matchPairIndices)
                            {
                                od.getKeypointsAndMatches(tk_ptr, &nTKeyPts, ik_ptr, &nIKeyPts, pair_ptr, &nMatchingPairs);
                            }
                        }
                    }
                }

                Debug.WriteLine(TAG + "==== Results of SURF descriptor calculation ====");
                Debug.WriteLine("TemplateKeyPoints: " + nTKeyPts + " ImageKeyPoints " + nIKeyPts + " nMatches " + nMatchingPairs);
                Debug.Write("Matching Indices: ");

                // template key points
                List<Point> tempKeyPoints = new List<Point>();
                for (int i = 0; i < nTKeyPts; i++)
                {
                    tempKeyPoints.Add(new Point((int) tKeyPoints[i].x, (int) tKeyPoints[i].y));
                }
                this.templateKeyPoints_ = tempKeyPoints;

                // image key points
                List<Point> imageKeyPoints = new List<Point>();
                for (int i = 0; i < nIKeyPts; i++)
                {
                    imageKeyPoints.Add(new Point((int)iKeyPoints[i].x, (int)iKeyPoints[i].y));
                }
                this.lastImageKeyPoints_ = imageKeyPoints;

               
                
                // point correspondences
                for (int i = 0; i < nMatchingPairs; i+=2)
                {
                    Debug.Write(matchPairIndices[i] + "," + matchPairIndices[i + 1] + " ");

                }

                // calculate list of matching points in input image
                List<Point> imageMatchingPoints = new List<Point>();

                // even are points in template, odd are points in camera image

                for (int i = 0; i < nMatchingPairs; i+=2)
                {
                    int t_idx = matchPairIndices[i];
                    int i_idx = matchPairIndices[i+1];

                    OpenCVDLL.SURFKeyPoint tPpt = tKeyPoints[t_idx];
                    OpenCVDLL.SURFKeyPoint iPpt = iKeyPoints[i_idx];

                    Point tPoint = new Point((int)tPpt.x, (int)tPpt.y);
                    Point iPoint = new Point((int)iPpt.x, (int)iPpt.y);

                    imageMatchingPoints.Add(tPoint);
                    imageMatchingPoints.Add(iPoint);

                }

                lastImageMatchingPoints_ = imageMatchingPoints;

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
                    lastCornerPoints_ = new Point[4];
                    int j = 0;
                    for (int k = 0; k < 4; k++)
                    {
                        Point cp = new Point(cornerPoints[j++], cornerPoints[j++]);
                        lastCornerPoints_[k] = cp;
                    }
                    
                }
                else
                {
                    lastCornerPoints_ = null; 
                }

               

                // TODO: --------------------- firing mechanism
                if (OnObjectDetected != null && imageMatchingPoints.Count>= DETECT_THRESH && frameCount > 20)
                {
                    OnObjectDetected(this, imageMatchingPoints);
                }
                // --------------------------------------------

            }



        }

    }
}
