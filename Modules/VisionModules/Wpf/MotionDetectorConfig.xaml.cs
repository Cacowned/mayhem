﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MayhemOpenCVWrapper;
using System.Diagnostics;
using System.Drawing;
using MayhemDefaultStyles.UserControls;

namespace VisionModules.Wpf
{
    /// <summary>
    /// Interaction logic for MotionDetectorConfig.xaml
    /// </summary>
    public partial class MotionDetectorConfig : IWpfConfiguration
    {
        public const string TAG = "[MotionDetectorConfig] :";
        private CameraDriver i = CameraDriver.Instance;
        protected Camera cam = null;
        public Camera selected_camera = null;
       

        private delegate void VoidHandler();

        // basically forward assignments to the actual overlay
        public Rect selectedBoundingRect
        {
            get 
            {
               return  overlay.GetBoundingRect();
            }
            set
            {
                overlay.DisplayBoundingRect(value);
            }
        }



        public MotionDetectorConfig()
        {
            InitializeComponent();
        }


        public override void OnLoad()
        {
          
            // populate device list

            Debug.WriteLine(TAG+"OnLoad");

            foreach (Camera c in i.cameras_available)
            {
                DeviceList.Items.Add(c);
            }

            DeviceList.SelectedIndex = 0;

            if (i.devices_available.Length > 0)
            {

                // start the camera 0 if it isn't already running
                cam = i.cameras_available[0];
                if (!cam.running) cam.StartFrameGrabbing();
                Debug.WriteLine(TAG + "using " + cam.info.ToString());
            }
            else
            {
                Debug.WriteLine(TAG + "No camera available");
            }

           // overlay.DisplayBoundingRect();
     
        }
       
         /**<summary>
         * Register / De-Register the image update handler if the window is visible / invisible
         * </summary>
         */ 
        private void Control_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Debug.WriteLine(TAG+"[MotionDetectorConfig] : IsVisibleChanged");
            if (this.IsVisible)
            {
                if (cam == null)
                {
                    OnLoad();
                }
                if (cam != null)
                {
                    if (!cam.running) cam.StartFrameGrabbing();
                    cam.OnImageUpdated += i_OnImageUpdated;
                }
            }
            else
            {
                if (cam != null)
                {
                    cam.OnImageUpdated -= i_OnImageUpdated;
                    cam.TryStopFrameGrabbing();
                }
                
            }
        }

        /**<summary>
         * Invokes image source setting when a new image is available from the update handler. 
         * </summary>
         */
        public void i_OnImageUpdated(object sender, EventArgs e)
        {
            Debug.WriteLine("[i_OnImageUpdated] ");
            Dispatcher.Invoke(new VoidHandler(SetCameraImageSource), null);
        }

        /**<summary>
         * Does the actual drawing of the camera image to the WPF image object in the config window. 
         * </summary>
         */ 
        public virtual void SetCameraImageSource()
        {
            Debug.WriteLine("[SetCameraImageSource] ");

            /*
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
            BackBuffer.UnlockBits(bmpData); */

            Bitmap BackBuffer = cam.ImageAsBitmap();

            IntPtr hBmp;

            //Convert the bitmap to BitmapSource for use with WPF controls
            hBmp = BackBuffer.GetHbitmap();

            this.camera_image.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBmp, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
           // this.camera_image.Source.Freeze();

            BackBuffer.Dispose();
            VisionModulesWPFCommon.DeleteObject(hBmp);


        }
     

        public override bool OnSave()
        {
            selected_camera = DeviceList.SelectedItem as Camera;
            // cam.OnImageUpdated -= imageUpdateHandler;

            cam = selected_camera;
            return true;
        }

       
    }
}
