﻿/* CamSnapshotConfig.xaml.cs
 * 
 * Snapshot configuration window using OpenCV Camera Library
 * 
 * Authors: Sven Kratz, Eli White
 * (c) 2011 Mayhem Open Source Project by Microsoft inc. 
 * 
 * 
 */

using System.Windows;
using System.Windows.Forms;
using MayhemOpenCVWrapper;
using System.Diagnostics;
using System.Drawing;
using System;
using MayhemDefaultStyles.UserControls;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Collections.ObjectModel;
using System.Windows.Media;
using Brush = System.Windows.Media.Brush; 
using Brushes = System.Windows.Media.Brushes;
using System.Windows.Data;
using System.Windows.Controls.Primitives; 
namespace VisionModules.Wpf
{

    /// <summary>
    /// Interaction logic for WebcamSnapshotConfig.xaml
    /// </summary>
    public partial class PictureConfig : IWpfConfiguration
    {
        public const string TAG = "[MotionDetectorConfig] :";
        public string location;
        // public Device captureDevice;

        public Camera selected_camera = null;

        private CameraDriver i = CameraDriver.Instance;
        protected List<Camera> cams = new List<Camera>();

        public ObservableCollection<ImageBrush> camera_previews = new ObservableCollection<ImageBrush>(); 

        // list of borders

        private List<Border> borders = new List<Border>();


        private delegate void ImageUpdateHandler(Camera c);

        //widths of the preview images
        private int preview_width = 0;
        private int preview_height = 0;

        // binding to the slider value 
        public double slider_value;

        // handle on brush (camera preview) that is currently selected
        private Border selected_preview_img = null;

        public PictureConfig(string location, double capture_offset_time)
        {
            this.location = location;
            slider_value = capture_offset_time;

            InitializeComponent();
            Init();
        }

        public  void Init()
        {
            // TODO: Enumerate devices

            // populate device list

            Debug.WriteLine(TAG + "Nr of Cameras available: " + i.cameras_available.Length);

            foreach (Camera c in i.cameras_available)
            {
                deviceList.Items.Add(c);
            }

            deviceList.SelectedIndex = 0;

            if (i.devices_available.Length > 0)
            {
                // start the camera 0 if it isn't already running
           
 
                // attach canvases with camera images to camera_preview_panel
                foreach (Camera c in i.cameras_available)
                {

                    cams.Add(c);

                    if (!c.running)
                    {
                        c.OnImageUpdated += i_OnImageUpdated;
                        c.StartFrameGrabbing();
                        Debug.WriteLine(TAG + "using " + c.info.ToString());
                        Debug.WriteLine(TAG + "Camera IDX " + c.index);
                    }

                    Debug.WriteLine("Adding...");                  
                    
                    //camera_preview_panel.Children.Add(canv);
                    int width = (int) ( camera_preview_panel.Width / 3);    
                    int height = (int)( width * 3 / 4);                     // 4:3 aspect ratio

                    preview_width = width;
                    preview_height = height; 

                 
                    // the camera previews are drawn onto an ImageBrush, which is shown in the 
                    // background of the DataTemplate
                    
                    camera_previews.Add(new ImageBrush());
                    

                }
                
                camera_preview_panel.ItemsSource = camera_previews;
                
            }
            else
            {
                Debug.WriteLine(TAG + "No camera available");
            }


            

            // capture offset slider
            int capture_size_s = Camera.LOOP_DURATION / 1000;
            slider_capture_offset.Minimum = -capture_size_s;
            slider_capture_offset.Maximum = capture_size_s;

            slider_capture_offset.IsDirectionReversed = false;
            slider_capture_offset.IsMoveToPointEnabled = true;
            slider_capture_offset.AutoToolTipPrecision = 2;
            slider_capture_offset.AutoToolTipPlacement =
              AutoToolTipPlacement.BottomRight;
            slider_capture_offset.TickPlacement = TickPlacement.TopLeft;

            slider_capture_offset.TickFrequency = 0.5;
            DoubleCollection tickMarks = new DoubleCollection();
            for (int k = -capture_size_s; k <= capture_size_s; k++)
            {
                tickMarks.Add(k);
            }

            slider_capture_offset.Ticks = tickMarks;

            slider_capture_offset.IsSelectionRangeEnabled = true;
            slider_capture_offset.SelectionStart = -capture_size_s;
            slider_capture_offset.SelectionEnd = capture_size_s;

            slider_capture_offset.SmallChange = 0.5;
            slider_capture_offset.LargeChange = 1.0;

            slider_capture_offset.Value = slider_value;

            /*
            slider_tickbar.Ticks = tickMarks;
            slider_tickbar.Minimum = -capture_size_s;
            slider_tickbar.Maximum = capture_size_s;*/
            


        }

        public override void OnClosing()
        {
            //cam.OnImageUpdated -= i_OnImageUpdated;
            //cam.TryStopFrameGrabbing();

            foreach (Camera c in cams)
            {
                c.OnImageUpdated -= i_OnImageUpdated;
                c.TryStopFrameGrabbing();
            }

        }

        ///<summary>
        /// Invokes image source setting when a new image is available from the update handler. 
        /// </summary>
        ///
        public void i_OnImageUpdated(object sender, EventArgs e)
        {
          
           Dispatcher.Invoke(new ImageUpdateHandler(SetCameraImageSource), sender);
            //SetCameraImageSource();
        }

        ///<summary>
        /// Does the actual drawing of the camera image(s) to the WPF image object in the config window. 
        ///</summary>       
        protected virtual void SetCameraImageSource(Camera cam)
        {
            
            Debug.WriteLine("[SetCameraImageSource] ");
            Debug.WriteLine("New Image on Camera " + cam.index +" : " + cam.info);

            Bitmap bm = cam.ImageAsBitmap();

            Bitmap shrink = ImageProcessing.ScaleWithFixedSize(bm, preview_width, preview_height);

  
            // convert bitmap to an hBitmap pointer and apply it as imagebrush imagesource
            IntPtr hBmp = shrink.GetHbitmap();
            BitmapSource s = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBmp, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            s.Freeze();
          
            camera_previews[cam.index].ImageSource = s;
          

            // dispose of all the unneeded data
            VisionModulesWPFCommon.DeleteObject(hBmp);
            shrink.Dispose();
            bm.Dispose();
            
            // update data binding
            BindingExpression bindingExpr = BindingOperations.GetBindingExpression(camera_preview_panel, ItemsControl.ItemsSourceProperty);
            if (bindingExpr != null)
            {
                bindingExpr.UpdateTarget();
            }
            else
            {
               // Debug.WriteLine("NULL!");
            }

        }

        /// <summary>
        /// Shows the dialog to select the file name of the template image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.SelectedPath = location;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                location = dlg.SelectedPath;
            }
        }

        public override bool OnSave()
        {
            selected_camera = deviceList.SelectedItem as Camera;
     
            return true;
        }

        public override string Title
        {
            get
            {
                return "Picture";
            }
        }

        /// <summary>
        ///  Register / De-Register the image update handler if the window is visible / invisible
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void root_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Debug.WriteLine(TAG + "IsVisibleChanged");
            if (this.IsVisible)
            {
          
                foreach (Camera c in cams)
                {
                    if (!c.running) c.StartFrameGrabbing();
                    c.OnImageUpdated += i_OnImageUpdated;
                }

            }
            else
            {
                foreach (Camera c in cams)
                {
                    c.OnImageUpdated -= i_OnImageUpdated;
                    c.TryStopFrameGrabbing();
                }
            }
        }

        #region Highlighting the camera preview selection 

        private void bborder_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Debug.WriteLine(TAG + "bborder_MouseLeftButtonUp");
            if (selected_preview_img != null)
            {
                selected_preview_img.BorderBrush = Brushes.Transparent;
            }

            Border b = sender as Border;
            b.BorderBrush = Brushes.Red;
            selected_preview_img = b; 

            // see which item we have selected
            ImageBrush brush = b.Background as ImageBrush;

            

            int item_index = camera_previews.IndexOf(brush);

            if (item_index >= 0)
            {
                Debug.WriteLine(TAG + "item index " + item_index);
                deviceList.SelectedIndex = item_index;
            }


        }
       

        private void deviceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selected_preview_img != null)
            {
                selected_preview_img.BorderBrush = Brushes.Transparent;
                int selected_index = deviceList.SelectedIndex;
               // selected_preview_img = camera_preview_panel.Items[selected_index] as Border;

                selected_preview_img = borders[selected_index];
                selected_preview_img.BorderBrush = Brushes.Red; 
            }
        }

        private void bborder_Loaded(object sender, RoutedEventArgs e)
        {
            Border b = sender as Border;
            borders.Add(b); 
        }

        private void bborder_Unloaded(object sender, RoutedEventArgs e)
        {
            Border b = sender as Border;
            borders.Remove(b);
        }

        #endregion

        private void slider_capture_offset_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            slider_value = slider_capture_offset.Value;
        }
    }
}