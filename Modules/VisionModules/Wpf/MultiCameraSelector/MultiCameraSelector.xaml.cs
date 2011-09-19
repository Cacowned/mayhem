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
using MayhemOpenCVWrapper;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using Brush = System.Windows.Media.Brush;
using Brushes = System.Windows.Media.Brushes;
using MayhemCore;

namespace VisionModules.Wpf
{
    /// <summary>
    /// Interaction logic for MultiCameraSelector.xaml
    /// </summary>
    public partial class MultiCameraSelector : UserControl
    {
        public Camera selected_camera = null;

        private CameraDriver i = null; //CameraDriver.Instance;
        protected List<Camera> cams = new List<Camera>();

        public ObservableCollection<ImageBrush> camera_previews = new ObservableCollection<ImageBrush>();

        private List<Border> borders = new List<Border>();

        private delegate void ImageUpdateHandler(Camera c);

        // widths of the preview images
        private int preview_width = 0;
        private int preview_height = 0;

        // handle on brush (camera preview) that is currently selected
        private Border selected_preview_img = null;

        public delegate void CameraSelectedHandler(Camera c);
        public event CameraSelectedHandler OnCameraSelected; 


        public MultiCameraSelector()
        {
            InitializeComponent();
            // avoid nasty errors in the designer due to not finding the OpenCVDLL
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                Init();
            }
        }

        private void Init()
        {
            i = CameraDriver.Instance;

            Logger.WriteLine("Nr of Cameras available: " + i.DeviceCount);
            foreach (Camera c in i.cameras_available)
            {
                deviceList.Items.Add(c);
            }

            deviceList.SelectedIndex = 0;

            if (i.DeviceCount > 0)
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
                        Logger.WriteLine("using " + c.info.ToString());
                        Logger.WriteLine("Camera IDX " + c.index);
                    }

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

            }
            else
            {
                Logger.WriteLine("No camera available");
                
            }
        }

        public  void OnClosing()
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
            Logger.WriteLine("New Image on Camera " + cam.index + " : " + cam.info);

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
                // Logger.WriteLine("NULL!");
            }

        }

        /// <summary>
        ///  Register / De-Register the image update handler if the window is visible / invisible
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void VisibilityChanged(bool visibility)
        {
            Logger.WriteLine("IsVisibleChanged");
            if (visibility)
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


        private void deviceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selected_preview_img != null)
            {
                selected_preview_img.BorderBrush = Brushes.Transparent;
                int selected_index = deviceList.SelectedIndex;
                // selected_preview_img = camera_preview_panel.Items[selected_index] as Border;

                selected_preview_img = borders[selected_index];
                selected_preview_img.BorderBrush = Brushes.Red;

                this.selected_camera = cams[selected_index];
                if (OnCameraSelected != null)
                {
                    OnCameraSelected(this.selected_camera);
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
