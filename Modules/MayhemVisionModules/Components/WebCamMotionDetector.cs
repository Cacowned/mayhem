using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows;
using System.Threading;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.IO;
using System.Drawing.Imaging;
using System.Windows.Threading;
using Emgu.CV;
using Emgu.Util;
using Emgu.CV.Structure;
using MayhemWebCamWrapper;
using System.Diagnostics;

namespace MayhemVisionModules.Components
{
    //performs motion detection... also stores motion detection template
    public class MotionDetection
    {
        public bool UpdateBackground(byte[] inImage, byte[] outImage, int width, int height, float detectorPercentage, float detectorDifference, float detectorTime, int roix, int roiy, int roiWidth, int roiHeight)
        {
            if (!initialized)
            {
                initTime = DateTime.Now;
            
                if (inputImage == null)
                {
                    inputImage = new Image<Bgr, Byte>(width, height);
                }

                if (segMask == null)
                {
                    segMask = new Image<Gray, Single>(width, height);
                }

                if (mhi == null || mhi.Width != width || mhi.Height != height)
                {
                    if (buff == null)
                    {
                        buff = new Image<Gray, Byte>[ringBufferSize];
                        for (int i = 0; i < ringBufferSize; i++)
                        {
                            buff[i] = new Image<Gray, Byte>(width, height);
                            buff[i].SetValue(0);
                        }
                    }

                    mhi = null;
                    mask = null;
                    mhi = new Image<Gray, Single>(width, height);
                    mhi.SetValue(0.0);
                    mask = new Image<Gray, Byte>(width, height);
                }

                initialized = true;
            }
            
            lastTime = DateTime.Now;
            TimeSpan t = lastTime.Subtract(initTime);
            double timestamp = t.TotalSeconds; 
            inputImage.Bytes = inImage;
            buff[last] = inputImage.Convert<Gray, Byte>();
            idx2 = (last + 1) % ringBufferSize;
            last = idx2;
            Image<Gray, Byte> silh = buff[idx2];
            Emgu.CV.CvInvoke.cvAbsDiff(buff[idx1].Ptr, buff[idx2].Ptr, silh.Ptr);
            Emgu.CV.CvInvoke.cvThreshold(silh.Ptr, silh.Ptr, detectorDifference, 1, Emgu.CV.CvEnum.THRESH.CV_THRESH_BINARY);
            Emgu.CV.CvInvoke.cvUpdateMotionHistory(silh.Ptr, mhi.Ptr, timestamp, detectorTime);
            double scale = 255.0 / detectorTime;
            Emgu.CV.CvInvoke.cvCvtScale(mhi.Ptr, mask.Ptr, scale, (detectorTime - timestamp) * scale);
            
            Rectangle motionRectangle = new Rectangle(roix, roiy, roiWidth, roiHeight);
            Emgu.CV.CvInvoke.cvSetImageROI(silh.Ptr, motionRectangle);
            double numMotionPixels = (double)Emgu.CV.CvInvoke.cvNorm(silh.Ptr, IntPtr.Zero, Emgu.CV.CvEnum.NORM_TYPE.CV_L1, IntPtr.Zero);
            double motionPercentage = 100.0*numMotionPixels / (motionRectangle.Width * motionRectangle.Height);

            //output
            Emgu.CV.CvInvoke.cvZero(origOutImage.Ptr);
            Emgu.CV.CvInvoke.cvMerge(IntPtr.Zero, IntPtr.Zero, mask.Ptr, IntPtr.Zero, origOutImage.Ptr);

            if (resizedOutImage == null)
            {
                resizedOutImage = new Image<Bgr, Byte>(width, height);
            }
            resizedOutImage = origOutImage.Resize(width, height, Emgu.CV.CvEnum.INTER.CV_INTER_LINEAR);
            if (motionPercentage > detectorPercentage && numWarmUpFrames > 30)
            {
                int centerLocationX = motionRectangle.X + motionRectangle.Width / 2;
                int centerLocationY = motionRectangle.Y + motionRectangle.Height / 2;
                int radius = (int)Math.Round(0.25*Math.Sqrt(motionRectangle.Width * motionRectangle.Width + motionRectangle.Height * motionRectangle.Height));
                System.Drawing.Point pt = new System.Drawing.Point(centerLocationX, centerLocationY);
                MCvScalar scalar = new MCvScalar(255,255,255);
                Emgu.CV.CvInvoke.cvCircle(resizedOutImage.Ptr, pt, radius, scalar, 2, Emgu.CV.CvEnum.LINE_TYPE.CV_AA, 0);
                isMotionDetected = true;
                numWarmUpFrames = 0;
            }
            ++numWarmUpFrames;
            Emgu.CV.CvInvoke.cvResetImageROI(silh);
            return true;
        }

        public bool IsMotionDetected()
        {
            bool ret = isMotionDetected;
            isMotionDetected = false;
            return ret;
        }

        int last = 0;
        int i, idx1 = 0, idx2;
        const int ringBufferSize = 4;
        DateTime initTime;
        DateTime lastTime;
        Image<Gray, Single> mhi = null;
        Image<Gray, Single> segMask = null;
        Image<Gray, Byte> mask = null;
        Image<Gray, Byte>[] buff;
        Image<Bgr, Byte> inputImage = null;
        Image<Bgr, Byte> origOutImage = new Image<Bgr, Byte>(640, 480);
        public Image<Bgr, Byte> resizedOutImage = null;
        bool initialized = false;
        int numWarmUpFrames = 0;
        bool isMotionDetected = false;
    }


    public class WebCamMotionDetector : ImageListener
    {
        public float MotionAreaPercentageSensitivity; //what percentage of pixel motion over the area is considered for a trigger? (between 0 and 100)
        public int MotionDiffSensitivity; //threshold used for background subtraction 
        public float TimeSensitivity; //how quickly should the motion detector adapt to time? (larger value means it takes longer to forget the past)
        public double RoiX, RoiY, RoiWidth, RoiHeight; //the region of interest over which processing will occur
        public int SelectedCameraIndex;
        private byte[] motionBuffer = null;
        private bool showBackground = true;
        MotionDetection Detector = new MotionDetection();


        public WebCamMotionDetector()
        {
            RoiX = 0;
            RoiY = 0;
            RoiWidth = 1;
            RoiHeight = 1;
            SelectedCameraIndex = -1;
            MotionAreaPercentageSensitivity = 5;
            MotionDiffSensitivity = 30;
            TimeSensitivity = 1;
        }

        ~WebCamMotionDetector()
        {
            Clear();
        }

        public void Clear()
        {
        }

        public void ToggleVisualization()
        {
            showBackground = !showBackground;
        }

        public event ComputerVisionImports.ComputerVisionEventHandler MotionDetected;
        // Invoke the motion detection event
        protected virtual void OnMotionDetected(EventArgs e)
        {
            if (MotionDetected != null)
                MotionDetected(this, e);
        }


        public override void UpdateFrame(object sender, EventArgs e)
        {
            WebCam camera = sender as WebCam;
            SelectedCameraIndex = camera.WebCamID;
            if (camera.ImageBuffer == null)
                return;


            if (ImagerWidth != default(double) && ImagerHeight != default(double) && SelectedCameraIndex > -1 && SelectedCameraIndex < WebcamManager.NumberConnectedCameras())
            {
                try
                {
                    if (motionBuffer == null || motionBuffer.Length != camera.ImageBuffer.Length)
                    {
                        motionBuffer = new byte[(int)(camera.ImageBuffer.Length)];
                    }
                    int scaledRoiX = Math.Max(Math.Min(Convert.ToInt32(ImagerWidth * RoiX), Convert.ToInt32(ImagerWidth)), 0);
                    int scaledRoiY = Math.Max(Math.Min(Convert.ToInt32(ImagerHeight * RoiY), Convert.ToInt32(ImagerHeight)), 0);
                    int scaledRoiWidth = Math.Max(Math.Min(Convert.ToInt32(ImagerWidth * RoiWidth), Convert.ToInt32(ImagerWidth)), 0);
                    int scaledRoiHeight = Math.Max(Math.Min(Convert.ToInt32(ImagerWidth * RoiHeight), Convert.ToInt32(ImagerHeight)), 0);
                    if (Detector.UpdateBackground(camera.ImageBuffer, motionBuffer, Convert.ToInt32(ImagerWidth), Convert.ToInt32(ImagerHeight), MotionAreaPercentageSensitivity, MotionDiffSensitivity, TimeSensitivity, scaledRoiX, scaledRoiY, scaledRoiWidth, scaledRoiHeight))
                    {
                        if (Detector.IsMotionDetected())
                            OnMotionDetected(EventArgs.Empty); // trigger event

                        if (showBackground)
                        {
                            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, (SendOrPostCallback)delegate
                            {
                                populateBitMap(Detector.resizedOutImage.Bytes, (int)(ImagerWidth * ImagerHeight * 3));
                                BitmapSource.Invalidate();
                                GC.Collect(); //this is due to a bug in InteropBitmap which causes memory leaks for 24 bit bitmaps... MS: FIX IT!
                            }, null);
                        }
                    }
                }
                catch (Emgu.CV.Util.CvException ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.ErrorMessage);
                }
            }
        }
        public override event PropertyChangedEventHandler PropertyChanged;
        protected override void NotifyPropertyChanged(string property)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(property));
            if (property == "SourceChanged" && ImagerWidth != default(double) && ImagerHeight != default(double))
            {
                uint numpixels = (uint)(ImagerWidth * ImagerHeight * PixelFormats.Bgr24.BitsPerPixel / 8);
                section = CreateFileMapping(new IntPtr(-1), IntPtr.Zero, 0x04, 0, numpixels, null);
                map = MapViewOfFile(section, 0xF001F, 0, 0, numpixels);
                BitmapSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromMemorySection(section, (int)ImagerWidth, (int)ImagerHeight, PixelFormats.Bgr24, (int)(ImagerWidth) * PixelFormats.Bgr24.BitsPerPixel / 8, 0) as InteropBitmap;
                Source = BitmapSource;

            }
        }

        void populateBitMap(byte[] data, int len)
        {

            if (ImagerWidth != default(int) && ImagerHeight != default(int))
            {
                Marshal.Copy(data, 0, map, len);
            }
        }

        public void SetImageSource(ImagerBase c)
        {
            if (SubscribedImagers.Count > 0)
            {
                RemoveImageSource(SubscribedImagers[0]);
            }
            Clear();
            WebCam camera = c as WebCam;
            SelectedCameraIndex = camera.WebCamID;
            RegisterForImages(c);
        }


        public void RemoveImageSource(ImagerBase c)
        {
            UnregisterForImages(c);
            SelectedCameraIndex = -1;
            Clear();
        }

        public InteropBitmap BitmapSource
        {
            get { return (InteropBitmap)GetValue(BitmapSourceProperty); }
            private set { SetValue(BitmapSourcePropertyKey, value); }
        }

        private static readonly DependencyPropertyKey BitmapSourcePropertyKey =
            DependencyProperty.RegisterReadOnly("BitmapSource", typeof(InteropBitmap), typeof(WebCamMotionDetector), new UIPropertyMetadata(default(InteropBitmap)));

        public static readonly DependencyProperty BitmapSourceProperty = BitmapSourcePropertyKey.DependencyProperty;

        //-----------------------------------------------------------------------------------------------------------------------------
        //cruft required for mapping received byte buffers from camera as interopbitmap instances
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateFileMapping(IntPtr hFile, IntPtr lpFileMappingAttributes, uint flProtect, uint dwMaximumSizeHigh, uint dwMaximumSizeLow, string lpName);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, uint dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, uint dwNumberOfBytesToMap);

        [DllImport("Kernel32.dll", EntryPoint = "RtlMoveMemory")]
        private static extern void CopyMemory(IntPtr Destination, IntPtr Source, int Length);
        //-----------------------------------------------------------------------------------------------------------------------------

        IntPtr section;
        IntPtr map { get; set; }
    }
}
