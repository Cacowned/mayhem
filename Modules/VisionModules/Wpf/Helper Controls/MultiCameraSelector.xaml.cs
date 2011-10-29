using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
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
    public partial class MultiCameraSelector : UserControl
    {
        public Camera SelectedCamera;
        public ObservableCollection<ImageBrush> CameraPreviews;

        private CameraDriver cameraDriver;

        private readonly List<Camera> cameras;
        private readonly List<Border> borders;

        private delegate void ImageUpdateHandler(Camera c);

        // widths of the preview images
        private int previewWidth;
        private int previewHeight;

        // handle on brush (camera preview) that is currently selected
        private Border selectedPreviewImg;

        public delegate void CameraSelectedHandler(Camera c);
        public event CameraSelectedHandler OnCameraSelected;

        public int SelectedIndex
        {
            get { return deviceList.SelectedIndex; }
        }

        public MultiCameraSelector()
        {
            InitializeComponent();

            CameraPreviews = new ObservableCollection<ImageBrush>();
            cameras = new List<Camera>();
            borders = new List<Border>();
        }

        internal void Init()
        {
            cameraDriver = CameraDriver.Instance;

            Logger.WriteLine("Nr of Cameras available: " + cameraDriver.DeviceCount);
            foreach (Camera c in cameraDriver.CamerasAvailable)
            {
                deviceList.Items.Add(c);
            }

            deviceList.SelectedIndex = 0;

            if (cameraDriver.DeviceCount > 0)
            {
                // attach canvases with camera images to camera_preview_panel
                foreach (Camera c in cameraDriver.CamerasAvailable)
                {
                    cameras.Add(c);
                    c.OnImageUpdated -= OnImageUpdated;
                    c.OnImageUpdated += OnImageUpdated;

                    c.StartFrameGrabbing();

                    Logger.WriteLine("using " + c.Info);
                    Logger.WriteLine("Camera IDX " + c.Index);
                    Logger.WriteLine("Adding...");

                    int width = (int)(camera_preview_panel.Width / 3);
                    int height = width * 3 / 4;                     // 4:3 aspect ratio

                    previewWidth = width;
                    previewHeight = height;

                    // the camera previews are drawn onto an ImageBrush, which is shown in the 
                    // background of the DataTemplate
                    CameraPreviews.Add(new ImageBrush());
                }

                camera_preview_panel.ItemsSource = CameraPreviews;
                SelectedCamera = cameras[0];
            }
            else
            {
                Logger.WriteLine("No camera available");
            }
        }

        private void OnClosing(object sender, RoutedEventArgs e)
        {
            foreach (Camera c in cameras)
            {
                c.OnImageUpdated -= OnImageUpdated;
                c.TryStopFrameGrabbing();
            }
        }

        /// <summary>
        /// Invokes image source setting when a new image is available from the update handler. 
        /// </summary>
        public void OnImageUpdated(object sender, EventArgs e)
        {
            Dispatcher.Invoke(new ImageUpdateHandler(SetCameraImageSource), sender);
        }

        /// <summary>
        /// Does the actual drawing of the camera image(s) to the WPF image object in the config window. 
        /// </summary>       
        protected virtual void SetCameraImageSource(Camera cam)
        {
            Bitmap bm = cam.ImageAsBitmap();
            Bitmap shrink = ImageProcessing.ScaleWithFixedSize(bm, previewWidth, previewHeight);

            // convert bitmap to an hBitmap pointer and apply it as imagebrush imagesource
            IntPtr hBmp = shrink.GetHbitmap();
            BitmapSource s = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBmp, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            s.Freeze();

            CameraPreviews[cam.Index].ImageSource = s;

            // dispose of all the unneeded data
            VisionModulesWpfCommon.DeleteGDIObject(hBmp);
            shrink.Dispose();
            bm.Dispose();

            // update data binding
            BindingExpression bindingExpr = BindingOperations.GetBindingExpression(camera_preview_panel, ItemsControl.ItemsSourceProperty);
            if (bindingExpr != null)
            {
                bindingExpr.UpdateTarget();
            }
        }

        #region Highlighting the camera preview selection

        private void bborder_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Logger.WriteLine("bborder_MouseLeftButtonUp");
            if (selectedPreviewImg != null)
            {
                selectedPreviewImg.BorderBrush = Brushes.Transparent;
            }

            Border b = sender as Border;
            b.BorderBrush = Brushes.Red;
            selectedPreviewImg = b;

            // see which item we have selected
            ImageBrush brush = b.Background as ImageBrush;

            int itemIndex = CameraPreviews.IndexOf(brush);

            if (itemIndex >= 0)
            {
                Logger.WriteLine("item index " + itemIndex);
                deviceList.SelectedIndex = itemIndex;
                SelectedCamera = cameras[itemIndex];
                if (OnCameraSelected != null)
                {
                    OnCameraSelected(SelectedCamera);
                }
            }
        }

        public void deviceList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (selectedPreviewImg != null)
            {
                selectedPreviewImg.BorderBrush = Brushes.Transparent;
                int selectedIndex = deviceList.SelectedIndex;
                selectedPreviewImg = camera_preview_panel.Items[selectedIndex] as Border;

                selectedPreviewImg = borders[selectedIndex];
                selectedPreviewImg.BorderBrush = Brushes.Red;

                SelectedCamera = cameras[selectedIndex];
                if (OnCameraSelected != null)
                {
                    OnCameraSelected(SelectedCamera);
                }
            }
            else
            {
                PresenceConfig c = sender as PresenceConfig;

                int selectedIndex = 0;
                if (c != null)
                {
                    selectedIndex = c.SelectedIndex;
                }

                // select a new preview image, if it is a sane alternative
                if (selectedIndex < borders.Count)
                {
                    selectedPreviewImg = borders[selectedIndex];
                    selectedPreviewImg.BorderBrush = Brushes.Red;
                    SelectedCamera = cameras[selectedIndex];
                    if (OnCameraSelected != null)
                    {
                        OnCameraSelected(SelectedCamera);
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
