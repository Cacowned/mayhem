using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
using MayhemCore;
using MayhemOpenCVWrapper;
using VisionModules.Components;

namespace VisionModules.Wpf
{
    /// <summary>
    /// Dialog window for the Face Detector
    /// Re-Uses CamSnaphostConfig, removing the save location dialog
    /// adding feedback of the face detector output
    /// </summary>
    public class FaceDetectConfig : MotionDetectorConfig
    {
        // use custom face detector to give feedback in the config window
        private FaceDetectorComponent fd;
        private List<System.Drawing.Point> faceDetectorPoints;
        private CameraDriver i = CameraDriver.Instance;
        private readonly List<int> amountOfFacesItems;
        private readonly ComboBox cbxNrFacesSelect;

        public int NumberOfFacesSelected
        {
            get
            {
                return (int)cbxNrFacesSelect.SelectedValue;
            }
        }

        public FaceDetectConfig(Camera c)
            : base(c)
        {
            faceDetectorPoints = new List<System.Drawing.Point>();
            amountOfFacesItems = new List<int>();

            // collapse the panel containing the sensitivity slider, which is not applicable to the face detector
            PnlSensitivity.Visibility = Visibility.Collapsed;

            // set up the face detector
            if (c != null)
            {
                fd = new FaceDetectorComponent(c);
                fd.OnFaceDetected += OnFaceDetected;
            }

            faceDetectorPoints = new List<System.Drawing.Point>();
          
            if (i.DeviceCount > 0)
                CanSave = true;

            // populate drop down list for amount of faces choice
            for (int k = 1; k <= 6; k++)
            {
                amountOfFacesItems.Add(k);
            }

            Label ddText = new Label();
            ddText.Content = "Event triggers when the following number of faces is detected:";

            StackPanel sp = new StackPanel();
            sp.Orientation = Orientation.Horizontal;

            ComboBox nFacesBox = new ComboBox();
            nFacesBox.ItemsSource = amountOfFacesItems;
            nFacesBox.SelectedIndex = 0;

            sp.Children.Add(ddText);
            sp.Children.Add(nFacesBox);

            pnl_additonal_widgets.Children.Add(sp);

            // retain the reference to the combobox
            cbxNrFacesSelect = nFacesBox;
        }

        /**<summary>
         * Does the actual drawing of the camera image to the WPF image object in the config window. 
         * </summary>
         */
        public override void SetCameraImageSource()
        {
            // send a frame to the detector
            Logger.WriteLine("Updating Face Detector Points");
            fd.UpdateFrame(SelectedCamera, null);

            Bitmap backBuffer = SelectedCamera.ImageAsBitmap();

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

                Graphics g = Graphics.FromImage(backBuffer);
                Color red = Color.Maroon;
                Brush b = new SolidBrush(red);
                Pen pen = new Pen(b);

                // mark the detected rectangles in the image
                for (int k = 0; k < points.Count; k += 2)
                {
                    Logger.Write("Drawing --- " + k);
                    int x = points[k].X;
                    int y = points[k].Y;
                    int w = points[k + 1].X - x;
                    int h = points[k + 1].Y - y;
                    Logger.Write(x + " " + y + " w " + w + " h " + h + " ");
                    g.DrawRectangle(pen, x, y, w, h);
                }

                Logger.WriteLine("Done");
            }

            // Convert the bitmap to BitmapSource for use with WPF controls
            IntPtr hBmp = backBuffer.GetHbitmap();

            // Finally display the image
            camera_image.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBmp, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());

            backBuffer.Dispose();
            VisionModulesWpfCommon.DeleteGDIObject(hBmp);
        }

        /**<summary>
         * Callback from the face detector
         * </summary>
         */
        private void OnFaceDetected(object sender, DetectionEventArgs pts)
        {
            List<System.Drawing.Point> points = pts.Points;
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
