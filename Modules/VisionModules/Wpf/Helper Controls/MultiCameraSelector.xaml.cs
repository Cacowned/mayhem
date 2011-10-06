/*
 *  MultiCameraSelector.xaml.cs
 * 
 *  Code-Behind for the multi-camera selector. 
 *  
 *  (c) 2011, Microsoft Applied Sciences Group
 * 
 *  Author: Sven Kratz
 * 
 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using MayhemCore;
using MayhemOpenCVWrapper;
using Brushes = System.Windows.Media.Brushes;

namespace VisionModules.Wpf
{
    /// <summary>
    /// Interaction logic for MultiCameraSelector.xaml
    /// </summary>
    public partial class MultiCameraSelector : UserControl
    {
        public Camera selected_camera = null;
        public ObservableCollection<ImageBrush> camera_previews = new ObservableCollection<ImageBrush>();

        private CameraDriver i = null; //CameraDriver.Instance;
        protected List<Camera> cams = new List<Camera>();
        private List<Border> borders = new List<Border>();
        private delegate void ImageUpdateHandler(Camera c);

        // widths of the preview images
        private int preview_width = 0;
        private int preview_height = 0;

        // handle on brush (camera preview) that is currently selected
        private Border selected_preview_img = null;

        public delegate void CameraSelectedHandler(Camera c);
        public event CameraSelectedHandler OnCameraSelected;

 

        private static readonly int DEBUG_LEVEL = 0;

        public int SelectedIndex
        {
            get {return deviceList.SelectedIndex;}
        }

        public MultiCameraSelector()
        {
            InitializeComponent();
        }

        internal void Init()
        {
            i = CameraDriver.Instance;

            Logger.WriteLine("Nr of Cameras available: " + i.DeviceCount);
            foreach (Camera c in i.CamerasAvailable)
            {
                deviceList.Items.Add(c);
            }

            deviceList.SelectedIndex = 0;

            if (i.DeviceCount > 0)
            {
                // attach canvases with camera images to camera_preview_panel
                foreach (Camera c in i.CamerasAvailable)
                {
                    cams.Add(c);
                    c.OnImageUpdated -= i_OnImageUpdated;
                    c.OnImageUpdated += i_OnImageUpdated;
                    
                    c.StartFrameGrabbing();
                  
                    Logger.WriteLine("using " + c.Info.ToString());
                    Logger.WriteLine("Camera IDX " + c.Index);                 
                    Logger.WriteLine("Adding...");

                    //camera_preview_panel.Children.Add(canv);
                    int width = (int)(camera_preview_panel.Width / 3);
                    int height = (int)(width * 3 / 4);                     // 4:3 aspect ratio

                    preview_width = width;
                    preview_height = height;

                    // the camera previews are drawn onto an ImageBrush, which is shown in the 
                    // background of the DataTemplate

                    camera_previews.Add(new ImageBrush());
                }

                camera_preview_panel.ItemsSource = camera_previews;
                selected_camera = cams[0];
            }
            else
            {
                Logger.WriteLine("No camera available");              
            }
        }


        private void OnClosing(object sender, RoutedEventArgs e)
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
        }

        ///<summary>
        /// Does the actual drawing of the camera image(s) to the WPF image object in the config window. 
        ///</summary>       
        protected virtual void SetCameraImageSource(Camera cam)
        {
           // Logger.WriteLineIf(DEBUG_LEVEL > 0,"New Image on Camera " + cam.Index + " : " + cam.Info);

            Bitmap bm = cam.ImageAsBitmap();
            Bitmap shrink = ImageProcessing.ScaleWithFixedSize(bm, preview_width, preview_height);

            // convert bitmap to an hBitmap pointer and apply it as imagebrush imagesource
            IntPtr hBmp = shrink.GetHbitmap();
            BitmapSource s = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBmp, IntPtr.Zero, Int32Rect.Empty, System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            s.Freeze();

            camera_previews[cam.Index].ImageSource = s;

            // dispose of all the unneeded data
            VisionModulesWPFCommon.DeleteGDIObject(hBmp);
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
                // Logger.WriteLine("NULL!");
            }
        }

        #region Highlighting the camera preview selection

        private void bborder_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Logger.WriteLine("bborder_MouseLeftButtonUp");
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
                Logger.WriteLine("item index " + item_index);
                deviceList.SelectedIndex = item_index;
                this.selected_camera = cams[item_index];
                if (OnCameraSelected != null)
                {
                    OnCameraSelected(this.selected_camera);
                }
            }

           


        }


        public void deviceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selected_preview_img != null)
            {
                selected_preview_img.BorderBrush = Brushes.Transparent;
                int selected_index = deviceList.SelectedIndex;
                selected_preview_img = camera_preview_panel.Items[selected_index] as Border;

                selected_preview_img = borders[selected_index];
                selected_preview_img.BorderBrush = Brushes.Red;

                this.selected_camera = cams[selected_index];
                if (OnCameraSelected != null)
                {
                    OnCameraSelected(this.selected_camera);
                }
            }
            else
            {
                PresenceConfig c = sender as PresenceConfig;

                int selected_index = 0;
                if (c != null)
                {
                    selected_index = c.selectedIndex;        
                }

                // select a new preview image, if it is a sane alternative
                if (selected_index < borders.Count)
                {
                    selected_preview_img = borders[selected_index];
                    selected_preview_img.BorderBrush = Brushes.Red;
                    this.selected_camera = cams[selected_index];
                    if (OnCameraSelected != null)
                    {
                        OnCameraSelected(this.selected_camera);
                    }
                }
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
    }
}
