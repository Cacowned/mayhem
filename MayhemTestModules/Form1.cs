using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using MayhemOpenCVWrapper;
using System.Windows;

using Point = System.Drawing.Point;

namespace MayhemTestModules
{
    public partial class Form1 : Form
    {

        private static Pen greenPen = new Pen(new SolidBrush(Color.ForestGreen));
        private static Pen redPen = new Pen(new SolidBrush(Color.Red));
        private static Pen whitePen = new Pen(new SolidBrush(Color.White));
        private static Pen orangeRedPen = new Pen(new SolidBrush(Color.OrangeRed));

        int c_width = 320;
        int c_height = 240;

        public MayhemImageUpdater imageUpdater;


        public MotionDetector m;

        public Form1()
        {

            //OpenCVDLL.OpenCVBindings.InitCapture(0,c_width, c_height);
            // OpenCVDLL.OpenCVBindings.StartCapturingFrames();

            imageUpdater = MayhemImageUpdater.Instance;

            imageUpdater.StartFrameGrabbing();

            m = new MotionDetector(c_width, c_height);

            m.OnMotionUpdate += new MotionDetector.MotionUpdateHandler(m_OnMotionUpdate);

          //  m.SetMotionBoundaryRect(new Rect(0, 0, 0, 0));


            InitializeComponent();

            

        }

        void m_OnMotionUpdate(object sender, List<Point> points)
        {
            //throw new NotImplementedException();

            Bitmap BackBuffer = new Bitmap(320, 240, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            Graphics g = Graphics.FromImage(BackBuffer);

            for (int i = 0; i < points.Count(); i+=2)
            {
                Point p1 = points[i];
                Point p2 = points[i + 1];

                g.DrawRectangle(orangeRedPen, new Rectangle(p1.X, p1.Y, p2.X - p1.X, p2.Y - p1.X));

                g.DrawRectangle(greenPen, new Rectangle(0,0,100,100));
            }

            pictureBox2.Image = BackBuffer;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            /*
            Bitmap BackBuffer = new Bitmap(320,240 , System.Drawing.Imaging.PixelFormat.Format24bppRgb);


            Rectangle rect = new Rectangle(0, 0, c_width, c_height);

            // get at the bitmap data in a nicer way
            System.Drawing.Imaging.BitmapData bmpData =
                BackBuffer.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                BackBuffer.PixelFormat);

            int bufSize = OpenCVDLL.OpenCVBindings.GetImageSize();

            IntPtr ImgPtr = bmpData.Scan0;


            byte[] buf = new byte[bufSize];

            // grab the image
            unsafe
            {
                fixed (byte* ptr = buf)
                {
                    OpenCVDLL.OpenCVBindings.GetNextFrame(ptr);

                }
            }

            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(buf, 0, ImgPtr, bufSize);

            // Unlock the bits.
            BackBuffer.UnlockBits(bmpData);


            pictureBox1.Image = BackBuffer;

            

            //OpenCVDLL.OpenCVBindings.GetNextFrame(buf);

            
            */

        }

        private void button2_Click(object sender, EventArgs e)
        {
            imageUpdater.OnImageUpdated += new MayhemImageUpdater.ImageUpdateHandler(imageUpdater_OnImageUpdated);

             m.RegisterForImages(imageUpdater);

        }

        void imageUpdater_OnImageUpdated(object sender, EventArgs e)
        {

            MayhemImageUpdater s = sender as MayhemImageUpdater;

            Bitmap BackBuffer = new Bitmap(320, 240, System.Drawing.Imaging.PixelFormat.Format24bppRgb);


              

                Rectangle rect = new Rectangle(0, 0, c_width, c_height);

                // get at the bitmap data in a nicer way
                System.Drawing.Imaging.BitmapData bmpData =
                    BackBuffer.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                    BackBuffer.PixelFormat);

                int bufSize = s.bufSize; //OpenCVDLL.OpenCVBindings.GetImageSize();

                IntPtr ImgPtr = bmpData.Scan0;

                lock (s.thread_locker)
                {


                // Copy the RGB values back to the bitmap
                System.Runtime.InteropServices.Marshal.Copy(s.imageBuffer, 0, ImgPtr, bufSize);

                // Unlock the bits.
                BackBuffer.UnlockBits(bmpData);

             }
                pictureBox1.Image = BackBuffer;
            
            
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            
            Bitmap BackBuffer = new Bitmap(320,240 , System.Drawing.Imaging.PixelFormat.Format24bppRgb);


            Rectangle rect = new Rectangle(0, 0, c_width, c_height);

            // get at the bitmap data in a nicer way
            System.Drawing.Imaging.BitmapData bmpData =
                BackBuffer.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                BackBuffer.PixelFormat);

            int bufSize = imageUpdater.bufSize;

            IntPtr ImgPtr = bmpData.Scan0;


          

            // grab the image
            

            lock(imageUpdater.thread_locker)
            {
            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(imageUpdater.imageBuffer, 0, ImgPtr, bufSize);
            }
            // Unlock the bits.
            BackBuffer.UnlockBits(bmpData);


            this.pictureBox2.Image = BackBuffer;

            

            //OpenCVDLL.OpenCVBindings.GetNextFrame(buf);

            
            
        }
    }
}
