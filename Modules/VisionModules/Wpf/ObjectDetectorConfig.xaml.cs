using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows;
using MayhemCore;
using MayhemOpenCVWrapper;
using MayhemWpf.UserControls;
using Point = System.Drawing.Point;
using VisionModules.Components;

namespace VisionModules.Wpf
{
    /// <summary>
    /// Object Detector Mayhem Module
    /// </summary>
    /// 
    public partial class ObjectDetectorConfig : WpfConfiguration
    {
        public string Location;

        public Camera SelectedCamera;

        private Camera camera;

        private delegate void VoidHandler();

        private readonly CameraDriver i;

        // object detector component for visualization use
        private ObjectDetectorComponent od;

        public Bitmap TemplateImg;
        public Bitmap TemplatePreview;

        private double templateScaleF;

        // basically forward assignments to the actual overlay
        public Rect SelectedBoundingRect
        {
            get
            {
                return overlay.GetBoundingRect();
            }
            set
            {
                overlay.DisplayBoundingRect(value);
            }
        }

        public ObjectDetectorConfig()
        {
            templateScaleF = 1.0;
            i = CameraDriver.Instance;
            InitializeComponent();
        }

        public override void OnLoad()
        {
            // TODO: camera resolution
            od = new ObjectDetectorComponent(320, 240);

            if (TemplateImg != null)
            {
                od.SetTemplate(TemplateImg);
            }

            foreach (Camera c in i.CamerasAvailable)
            {
                DeviceList.Items.Add(c);
            }

            DeviceList.SelectedIndex = 0;

            if (i.DeviceCount > 0)
            {
                // start the camera 0 if it isn't already running
                camera = i.CamerasAvailable[0];
                if (!camera.Running)
                {
                    camera.OnImageUpdated += OnImageUpdated;
                    ThreadPool.QueueUserWorkItem(o => camera.StartFrameGrabbing());
                }

                Logger.WriteLine("using " + camera.Info);
            }
            else
            {
                Logger.WriteLine("No camera available");
            }
        }

        public override void OnClosing()
        {
            camera.OnImageUpdated -= OnImageUpdated;
            camera.TryStopFrameGrabbing();
        }

        /**<summary>
        * Invokes image source setting when a new image is available from the update handler. 
        * </summary>
        */
        public void OnImageUpdated(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new VoidHandler(SetCameraImageSource), null);
        }

        /**<summary>
         * Does the actual drawing of the camera image to the WPF image object in the config window. 
         * </summary>
         */
        protected virtual void SetCameraImageSource()
        {
            Bitmap backBuffer = new Bitmap(320, 240, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            Rectangle rect = new Rectangle(0, 0, 320, 240);

            // get at the bitmap data in a nicer way
            System.Drawing.Imaging.BitmapData bmpData =
                backBuffer.LockBits(
                    rect,
                    System.Drawing.Imaging.ImageLockMode.ReadWrite,
                    backBuffer.PixelFormat);

            int bufSize = camera.BufferSize;

            IntPtr imgPtr = bmpData.Scan0;

            // grab the image
            // Copy the RGB values back to the bitmap
            System.Runtime.InteropServices.Marshal.Copy(camera.ImageBuffer, 0, imgPtr, bufSize);

            // Unlock the bits.
            backBuffer.UnlockBits(bmpData);

            // if a template preview is available, overlay with camera image
            if (TemplatePreview != null)
            {
                // do some actual object detection 
                od.UpdateFrame(camera, null);

                List<Point> matches = od.LastImageMatchingPoints;
                List<Point> tKeyPts = od.TemplateKeyPoints;
                List<Point> iKeyPts = od.LastImageKeyPoints;
                Point[] corners = od.LastCornerPoints;

                Logger.WriteLine("SetCameraImageSource --> Drawing Overlay");
                Graphics g = Graphics.FromImage(backBuffer);
                if (TemplatePreview != null)
                    g.DrawImage(TemplatePreview, 0, 0);

                if (matches != null)
                {
                    Logger.WriteLine("Matching Points:");
                    foreach (Point p in matches)
                    {
                        Logger.Write(p + " , ");
                    }
                }

                // draw template keypoints (hScalef and wScalef must be correct!)
                if (tKeyPts != null)
                {
                    foreach (Point p in tKeyPts)
                    {
                        double wScalef = (double)TemplatePreview.Width / TemplateImg.Width;
                        double hScalef = (double)TemplatePreview.Height / TemplateImg.Height;

                        int x = (int)(p.X * wScalef);
                        int y = (int)(p.Y * hScalef);

                        const int r = 2;
                        if (x - r >= 0 && y - r >= 0)
                        {
                            g.DrawRectangle(Pens.Red, x - r, y - r, r, r);
                        }
                        else
                        {
                            g.DrawRectangle(Pens.Red, x, y, r, r);
                        }
                    }
                }

                // draw matches in camera image (if present) and line do correspondences in 
                // camera image
                if (matches != null)
                {
                    for (int k = 0; k < matches.Count; k += 2)
                    {
                        Point tPt = matches[k];
                        Point iPt = matches[k + 1];

                        double wScalef = (double)TemplatePreview.Width / TemplateImg.Width;
                        double hScalef = (double)TemplatePreview.Height / TemplateImg.Height;

                        int tx = (int)(tPt.X * wScalef);
                        int ty = (int)(tPt.Y * hScalef);

                        const int r = 5;
                        if (tx - r >= 0 && ty - r >= 0)
                        {
                            g.DrawRectangle(Pens.Green, tx - r, ty - r, r, r);
                        }
                        else
                        {
                            g.DrawRectangle(Pens.Green, tx, ty, r, r);
                        }

                        int ix = iPt.X;
                        int iy = iPt.Y;

                        if (ix - r >= 0 && iy - r >= 0)
                        {
                            g.DrawRectangle(Pens.Green, ix - r, iy - r, r, r);
                        }
                        else
                        {
                            g.DrawRectangle(Pens.Green, ix, iy, r, r);
                        }

                        g.DrawLine(Pens.GreenYellow, tx, ty, ix, iy);
                    }
                }

                // draw rectangle of object bounding corner
                if (corners != null)
                {
                    g.DrawLine(Pens.LightBlue, corners[0], corners[1]);
                    g.DrawLine(Pens.LightBlue, corners[1], corners[2]);
                    g.DrawLine(Pens.LightBlue, corners[2], corners[3]);
                    g.DrawLine(Pens.LightBlue, corners[3], corners[0]);
                }

                g.Dispose();
            }

            // Convert the bitmap to BitmapSource for use with WPF controls
            IntPtr hBmp = backBuffer.GetHbitmap();

            camera_image.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBmp, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            camera_image.Source.Freeze();

            backBuffer.Dispose();
            VisionModulesWpfCommon.DeleteGDIObject(hBmp);
        }

        /// <summary>
        /// Method gets called when the save button is clicked
        /// </summary>
        public override void OnSave()
        {
            Logger.WriteLine("OnSave!!!!!!!!");
            SelectedCamera = DeviceList.SelectedItem as Camera;

            camera = SelectedCamera;
            if (TemplateImg != null)
            {
                if (TemplateImg.PixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb)
                {
                    // Todo: Change this --> should go into the event OnSave
                    // this.objectDetectorEvent.setTemplateImage(this.templateImg);
                    // this.objectDetectorEvent.templatePreview = templatePreview;
                    Logger.WriteLine("OnSave --> successfully assigned template image");
                }
            }
        }

        public override string Title
        {
            get
            {
                return "Object Detector";
            }
        }

        private void btn_templateFromFile_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "Image Files (*.bmp, *.gif, *.exif, *.jpg, *.png, *.tiff)|*.bmp;*.gif;*.exif;*.jpg;*.png;*.tiff";
            dlg.Title = "Select Template Image File";
            bool? result = dlg.ShowDialog();

            if (result == true)
            {
                string fileName = dlg.FileName;

                Bitmap tImg;

                // TODO: Better Exception Handling --> what if the file isn't an image !?
                try
                {
                    tImg = new Bitmap(fileName);
                    if (tImg.PixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb)
                    {
                        TemplateImg = tImg;

                        // scale down the template image to generate a preview image
                        int w = TemplateImg.Width;
                        int h = TemplateImg.Height;
                        templateScaleF = 100.0 / w;
                        Bitmap preview = ImageProcessing.ScaleWithFixedSize(TemplateImg, 100, (int)(h * templateScaleF));
                        TemplatePreview = preview;
                        od.SetTemplate(TemplateImg);
                    }
                    else
                    {
                        System.Windows.Forms.MessageBox.Show("The image color format is not supported. You need to provide an 24bit rgb color image", "Mayhem", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                        return;
                    }
                }
                catch (FileNotFoundException)
                {
                    System.Windows.Forms.MessageBox.Show("The image file you specified could not be found", "Mayhem", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                    return;
                }

                // pass template bitmap to objectDetectorEvent
                if (tImg != null)
                {
                    // TODO
                }

                Logger.WriteLine("Image Loaded Successfully");

                // update helper text
                helperText.Content = "image set (event is now ready to work)";
                helperText.Foreground = System.Windows.Media.Brushes.DarkGreen;
            }
            else
            {
                // System.Windows.Forms.MessageBox.Show("File Selection Error", "Mayhem", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                Logger.WriteLine("user clicked cancel");
                return;
            }
        }

        private void btn_templateFromRegion_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
