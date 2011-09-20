/*   FaceDetectConfig
 *   Dialog window for the Face Detector
 *   Re-Uses CamSnaphostConfig, removing the save location dialog
 *   adding feedback of the face detector output 
 */ 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using MayhemOpenCVWrapper.LowLevel;
using MayhemCore;
using MayhemOpenCVWrapper;


namespace VisionModules.Wpf
{
    public  class FaceDetectConfig : MotionDetectorConfig
    {
        // use custom face detector to give feedback in the config window
        private FaceDetectorComponent fd;
        private FaceDetectorComponent.DetectionHandler faceDetectUpdateHandler;

        List<System.Drawing.Point> faceDetectorPoints = new List<System.Drawing.Point>();

        private CameraDriver i = CameraDriver.Instance;


        public FaceDetectConfig(Camera c) : base(c)
           
        {
            

            // set up the face detector
            fd = new FaceDetectorComponent();
            faceDetectUpdateHandler = new FaceDetectorComponent.DetectionHandler(m_onFaceDetected);
            faceDetectorPoints = new List<System.Drawing.Point>();

            fd.OnFaceDetected += m_onFaceDetected;
            if (i.DeviceCount > 0)
                CanSave = true;



        }

        /**<summary>
         * Does the actual drawing of the camera image to the WPF image object in the config window. 
         * </summary>
         */
        public override void SetCameraImageSource()
        {
            // send a frame to the detector
            Logger.WriteLine("Updating Face Detector Points");
            fd.update_frame(cam, null);

            //int stride = 320 * 3;
            Bitmap BackBuffer = new Bitmap(320, 240, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, 320, 240);

            // get at the bitmap data in a nicer way
            System.Drawing.Imaging.BitmapData bmpData =
                BackBuffer.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                BackBuffer.PixelFormat);

            int bufSize = cam.bufSize;

            IntPtr ImgPtr = bmpData.Scan0;

            // grab the image


            lock (cam.thread_locker)
            {
                // Copy the RGB values back to the bitmap
                System.Runtime.InteropServices.Marshal.Copy(cam.imageBuffer, 0, ImgPtr, bufSize);
            }
            // Unlock the bits.
            BackBuffer.UnlockBits(bmpData);

            IntPtr hBmp;

            // mark face points if face has been detected
            if (faceDetectorPoints.Count > 0)
            {
                Logger.WriteLine("Drawing Face Detector Points..."); 

                // make a local copy of the detector points
                List<System.Drawing.Point> points = new List<System.Drawing.Point>();
                lock (this)
                {
                    // copy over to a local buffer to allow async updates
                    foreach (System.Drawing.Point p in faceDetectorPoints)
                    {
                        points.Add(p);
                    }
                }

                Graphics g = Graphics.FromImage(BackBuffer);
                Color red = Color.Maroon;
                Brush b = new SolidBrush(red);
                Pen pen = new Pen(b);

                // mark the detected rectangles in the image

                for (int k = 0; k < points.Count; k+=2)
                {
                    Logger.Write("Drawing --- " + k);
                    int x = points[k].X;
                    int y = points[k].Y;
                    int w = points[k + 1].X-x;
                    int h = points[k+1].Y-y;
                    Logger.Write(x + " " + y + " w " + w + " h " + h + " " );
                    g.DrawRectangle(pen, x, y, w, h);
                }
            

                Logger.WriteLine("Done");


            }
            

            //Convert the bitmap to BitmapSource for use with WPF controls
            hBmp = BackBuffer.GetHbitmap();

            // Finally display the image
            this.camera_image.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBmp, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            // this.camera_image.Source.Freeze();

            BackBuffer.Dispose();
            VisionModulesWPFCommon.DeleteObject(hBmp);


        }

        /**<summary>
         * Callback from the face detector
         * </summary>
         */ 
        void m_onFaceDetected(object sender, List<System.Drawing.Point> points)
        {
            Logger.WriteLine("Got Points from Face Detector!");
            // update the points in a synchronized block
            lock (this)
            {
                faceDetectorPoints = new List<System.Drawing.Point>();
                foreach (System.Drawing.Point p in points)
                {
                    faceDetectorPoints.Add(p);
                }
            }
        }

        public override string Title
        {
            get
            {
                return "Face Detector";
            }
        }
    }
}
