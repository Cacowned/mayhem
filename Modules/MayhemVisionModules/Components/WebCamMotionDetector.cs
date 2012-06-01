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
using MayhemWebCamWrapper;

namespace MayhemVisionModules.Components
{
    //performs motion detection... also stores motion detection template
    public class WebCamMotionDetector : ImageListener
    {
        public float MotionAreaPercentageSensitivity; //what percentage of pixel motion over the area is considered for a trigger? (between 0 and 100)
        public int MotionDiffSensitivity; //threshold used for background subtraction 
        public float TimeSensitivity; //how quickly should the motion detector adapt to time? (larger value means it takes longer to forget the past)
        public double RoiX, RoiY, RoiWidth, RoiHeight; //the region of interest over which processing will occur
        public int SelectedCameraIndex;
        private byte[] motionBuffer = new byte[(int)(640 * 480 * 3)];
        private bool showBackground = true;
        private IntPtr pMotionDetector = ComputerVisionImports.CreateMotionDetector();
   


        public WebCamMotionDetector()
        {
            RoiX = 0;
            RoiY = 0;
            RoiWidth = 640;
            RoiHeight = 480;
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
            if (pMotionDetector != IntPtr.Zero)
            {
                ComputerVisionImports.DisposeMotionDetector(pMotionDetector);
                pMotionDetector = IntPtr.Zero;
            }
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
            if (pMotionDetector == IntPtr.Zero)
            {
               pMotionDetector = ComputerVisionImports.CreateMotionDetector();
            }
            if (ImagerWidth != default(double) && ImagerHeight != default(double) && SelectedCameraIndex > -1 && SelectedCameraIndex < WebcamManager.NumberConnectedCameras() && pMotionDetector != IntPtr.Zero)
            {
                try
                {
                    int scaledRoiX = Math.Max(Math.Min(Convert.ToInt32(ImagerWidth*RoiX),Convert.ToInt32(ImagerWidth)),0) ;
                    int scaledRoiY = Math.Max(Math.Min(Convert.ToInt32(ImagerHeight*RoiY), Convert.ToInt32(ImagerHeight)), 0);
                    int scaledRoiWidth = Math.Max(Math.Min(Convert.ToInt32(ImagerWidth*RoiWidth), Convert.ToInt32(ImagerWidth)), 0);
                    int scaledRoiHeight = Math.Max(Math.Min(Convert.ToInt32(ImagerWidth*RoiHeight), Convert.ToInt32(ImagerHeight)), 0);
                    //(Convert.ToInt32(ImagerHeight) - scaledRoiY) - scaledRoiHeight
                    ComputerVisionImports.UpdateBackground(pMotionDetector, camera.ImageBuffer, showBackground ? motionBuffer : null, Convert.ToInt32(ImagerWidth), Convert.ToInt32(ImagerHeight), MotionAreaPercentageSensitivity, MotionDiffSensitivity, TimeSensitivity, scaledRoiX, scaledRoiY, scaledRoiWidth, scaledRoiHeight);
                  
                    if (ComputerVisionImports.IsMotionDetected(pMotionDetector))
                        OnMotionDetected(EventArgs.Empty); // trigger event

                    if (showBackground)
                    {
                        populateBitMap(motionBuffer, (int)(ImagerWidth * ImagerHeight * 3));

                        Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, (SendOrPostCallback)delegate
                        {
                            BitmapSource.Invalidate();
                            GC.Collect(); //this is due to a bug in InteropBitmap which causes memory leaks for 24 bit bitmaps... MS: FIX IT!
                        }, null);
                    }
                }
                catch
                {
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
