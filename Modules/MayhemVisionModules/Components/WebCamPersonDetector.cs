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
    public class PersonDetection
    {
        public bool DetectPerson(byte[] inImage, int width, int height,  int roix, int roiy, int roiWidth, int roiHeight)
        {
            if (cvInImage == null || cvInImage.Width != width || cvInImage.Height != height)
            {
                cvInImage = null;
                cvInImage = new Image<Bgr, Byte>(width, height);
            }

            if (cvOutImage == null || cvOutImage.Width != width || cvOutImage.Height != height)
            {
                cvOutImage = null;
                cvOutImage = new Image<Bgr, Byte>(width, height);
            }
            if (!initialized)
            {
                hog.SetSVMDetector(HOGDescriptor.GetDefaultPeopleDetector());
                initialized = true;
            }

            cvInImage.Bytes = inImage;
            cvOutImage.Bytes = inImage;
            System.Drawing.Size window = new System.Drawing.Size(8,8);
            System.Drawing.Size padding = new System.Drawing.Size(32,32);
            Rectangle[] rects = hog.DetectMultiScale(cvInImage);
            foreach (Rectangle pedestrain in rects)
            {
                cvOutImage.Draw(pedestrain, new Bgr(System.Drawing.Color.Red), 5);
            }
            return false;
        }

        private Image<Bgr, Byte> cvInImage = null;
        public Image<Bgr, Byte> cvOutImage = null;
        private HOGDescriptor hog = new HOGDescriptor();
        bool initialized = false;
    }

    public class WebCamPersonDetector : ImageListener
    {
        public double RoiX, RoiY, RoiWidth, RoiHeight; //the region of interest over which processing will occur
        public int SelectedCameraIndex;
        private bool showDebug = true;
        PersonDetection Detector = new PersonDetection();


        public WebCamPersonDetector()
        {
            RoiX = 0;
            RoiY = 0;
            RoiWidth = 1;
            RoiHeight = 1;
            SelectedCameraIndex = -1;
        }

        ~WebCamPersonDetector()
        {
            Clear();
        }

        public void Clear()
        {
        }

        public void ToggleVisualization()
        {
            showDebug = !showDebug;
        }

        public event ComputerVisionImports.ComputerVisionEventHandler PersonDetected;
        // Invoke the motion detection event
        protected virtual void OnPersonDetected(EventArgs e)
        {
            if (PersonDetected != null)
                PersonDetected(this, e);
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
                    int scaledRoiX = Math.Max(Math.Min(Convert.ToInt32(ImagerWidth * RoiX), Convert.ToInt32(ImagerWidth)), 0);
                    int scaledRoiY = Math.Max(Math.Min(Convert.ToInt32(ImagerHeight * RoiY), Convert.ToInt32(ImagerHeight)), 0);
                    int scaledRoiWidth = Math.Max(Math.Min(Convert.ToInt32(ImagerWidth * RoiWidth), Convert.ToInt32(ImagerWidth)), 0);
                    int scaledRoiHeight = Math.Max(Math.Min(Convert.ToInt32(ImagerWidth * RoiHeight), Convert.ToInt32(ImagerHeight)), 0);
                    if (Detector.DetectPerson(camera.ImageBuffer, Convert.ToInt32(ImagerWidth), Convert.ToInt32(ImagerHeight), scaledRoiX, scaledRoiY, scaledRoiWidth, scaledRoiHeight))
                    {
                        OnPersonDetected(EventArgs.Empty); // trigger event
                    }

                    if (showDebug)
                    {
                        Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, (SendOrPostCallback)delegate
                        {
                            populateBitMap(Detector.cvOutImage.Bytes, (int)(ImagerWidth * ImagerHeight * 3));
                            BitmapSource.Invalidate();
                            GC.Collect(); //this is due to a bug in InteropBitmap which causes memory leaks for 24 bit bitmaps... MS: FIX IT!
                        }, null);
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
