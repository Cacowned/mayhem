/*
 *  ObjectDetectorEvent.cs
 * 
 *  Object Detector Mayhem Module
 * 
 * (c) Microsoft Applied Sciences Group, 2011
 * 
 *  Author: Sven Kratz
 * 
 */  

using System;
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
using MayhemDefaultStyles.UserControls;
using MayhemOpenCVWrapper;
using System.Diagnostics;
using System.Drawing;
using VisionModules.Events;
using System.IO;
using Image=System.Drawing.Image;
using Point = System.Drawing.Point;
using System.Drawing.Drawing2D;
using MayhemOpenCVWrapper.LowLevel;


namespace VisionModules.Wpf
{
    /// <summary>
    /// Interaction logic for ObjectDetectorConfig.xaml
    /// </summary>
    /// 

    
    
    public partial class ObjectDetectorConfig : IWpfConfiguration
    {
        public const string TAG = "[VisionModules.Wpf.ObjectDetectorConfig] :";
        public string location;

        public Camera selected_camera = null;

        protected Camera cam = null;

        private delegate void VoidHandler();

        private CameraDriver i = CameraDriver.Instance;

        // handle on the event that will be configured
        private ObjectDetectorEvent objectDetectorEvent;

        // object detector component for visualization use
        private ObjectDetectorComponent od;

        public Bitmap templateImg = null;
        public Bitmap templatePreview = null ;

        private double template_scale_f = 1.0;

        // basically forward assignments to the actual overlay
        public Rect selectedBoundingRect
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
 
        

        public ObjectDetectorConfig(ObjectDetectorEvent objDetector, object captureDevice)
        {
            objectDetectorEvent = objDetector;
            InitializeComponent();

            // TODO: camera resolution
            od = new ObjectDetectorComponent(320, 240);

            if (templateImg != null)
            {
                od.set_template(templateImg);
            }
            


        }

        public override void OnLoad()
        {
            foreach (Camera c in i.cameras_available)
            {
                DeviceList.Items.Add(c);
            }

            DeviceList.SelectedIndex = 0;

            if (i.devices_available.Length > 0)
            {
                // start the camera 0 if it isn't already running
                cam = i.cameras_available[0];
                if (!cam.running)
                {
                    cam.OnImageUpdated += i_OnImageUpdated;
                    cam.StartFrameGrabbing();
                }
                Debug.WriteLine(TAG + "using " + cam.info.ToString());
            }
            else
            {
                Debug.WriteLine(TAG + "No camera available");
            }


        }

        public override void OnClosing()
        {
            cam.OnImageUpdated -= i_OnImageUpdated;
            cam.TryStopFrameGrabbing();
        }

        /**<summary>
        * Invokes image source setting when a new image is available from the update handler. 
        * </summary>
        */
        public void i_OnImageUpdated(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new VoidHandler(SetCameraImageSource), null);
        }

        /**<summary>
         * Does the actual drawing of the camera image to the WPF image object in the config window. 
         * </summary>
         */
        protected virtual void SetCameraImageSource()
        {
            //Debug.WriteLine("[SetCameraImageSource] ");

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


            // if a template preview is available, overlay with camera image
            if (templatePreview != null )
            {
                // do some actual object detection 
                
                // Bitmap cameraImage = new Bitmap(BackBuffer);

                od.update_frame(cam, null);
                

                List<Point> matches = od.lastImageMatchingPoints;
                List<Point> tKeyPts = od.templateKeyPoints;
                List<Point> iKeyPts = od.lastImageKeyPoints;
                Point[] corners = od.lastCornerPoints;

                Debug.WriteLine(TAG + "SetCameraImageSource --> Drawing Overlay");
                Graphics g = Graphics.FromImage(BackBuffer);
                if (templatePreview != null)
                    g.DrawImage(templatePreview, 0, 0);

                if (matches != null )
                {
                    Debug.WriteLine("Matching Points:");
                    foreach (Point p in matches)
                    {
                        Debug.Write(p + " , ");
                    }
                    Debug.WriteLine("");
                }

                // draw template keypoints (hScalef and wScalef must be correct!)
                if (tKeyPts != null)
                {

                    foreach (Point p in tKeyPts)
                    {
                        double wScalef = (double)templatePreview.Width / (double)templateImg.Width;
                        double hScalef = (double)templatePreview.Height / (double)templateImg.Height;


                        int x = (int)((double)p.X * wScalef);
                        int y = (int)((double)p.Y * hScalef);

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

                        double wScalef = (double)templatePreview.Width / (double)templateImg.Width;
                        double hScalef = (double)templatePreview.Height / (double)templateImg.Height;


                        int tx = (int)((double)tPt.X * wScalef);
                        int ty = (int)((double)tPt.Y * hScalef);

                        const int r = 5;
                        if (tx - r >= 0 && ty - r >= 0)
                        {
                            g.DrawRectangle(Pens.Green, tx - r, ty - r, r, r);
                        }
                        else
                        {
                            g.DrawRectangle(Pens.Green, tx, ty, r, r);
                        }

                        int ix = (int)((double)iPt.X);
                        int iy = (int)((double)iPt.Y);


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
            
            IntPtr hBmp;

            //Convert the bitmap to BitmapSource for use with WPF controls
            hBmp = BackBuffer.GetHbitmap();

            this.camera_image.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBmp, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            this.camera_image.Source.Freeze();


            BackBuffer.Dispose();
            VisionModulesWPFCommon.DeleteObject(hBmp);
        }

        /**<summary>
         * Method gets called when the save button is clicked
         * </summary> */
         public override bool OnSave()
        {
            Debug.WriteLine(TAG + "OnSave!!!!!!!!");
            selected_camera = DeviceList.SelectedItem as Camera;
            // cam.OnImageUpdated -= imageUpdateHandler;

            cam = selected_camera;
            if (this.templateImg != null)
            {
                if (this.templateImg.PixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb)
                {
                    this.objectDetectorEvent.setTemplateImage(this.templateImg);
                    this.objectDetectorEvent.templatePreview = templatePreview;
                    Debug.WriteLine(TAG + "OnSave --> successfully assigned template image");
                }
                
            }
            return true;
        }

           /// <summary>
           /// Register / De-Register the image update handler if the window is visible / invisible
           /// </summary>
            private void Control_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
            {
                Debug.WriteLine(TAG + "[MotionDetectorConfig] : IsVibleChanged");
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
            Nullable<bool> result = dlg.ShowDialog();

            if (result == true)
            {
                string fileName = dlg.FileName;

                Bitmap tImg = null; 
                // TODO: Better Exception Handling --> what if the file isn't an image !?
                try
                {
                     tImg = new Bitmap(fileName);
                     if (tImg.PixelFormat == System.Drawing.Imaging.PixelFormat.Format24bppRgb)
                     {
                         this.templateImg = tImg;
                         // scale down the template image to generate a preview image
                         int w = templateImg.Width;
                         int h = templateImg.Height;
                         this.template_scale_f = 100.0 / w;
                         Bitmap preview = ImageProcessing.ScaleWithFixedSize(templateImg, 100, (int)( h * template_scale_f));
                         this.templatePreview = preview;  
                         od.set_template(templateImg);
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
                   
                }

                Debug.WriteLine(TAG + "Image Loaded Successfully");

                // update helper text
                helperText.Content = "image set (event is now ready to work)";
                helperText.Foreground = System.Windows.Media.Brushes.DarkGreen;
             
            }
            else
            {
               // System.Windows.Forms.MessageBox.Show("File Selection Error", "Mayhem", System.Windows.Forms.MessageBoxButtons.OK, System.Windows.Forms.MessageBoxIcon.Error);
                Debug.WriteLine(TAG + "user clicked cancel");
                return;
            }         
        }

        

        private void btn_templateFromRegion_Click(object sender, RoutedEventArgs e)
        {
            throw new NotImplementedException();
        }

    
    }
}
