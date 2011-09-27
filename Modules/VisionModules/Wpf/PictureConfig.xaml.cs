/* CamSnapshotConfig.xaml.cs
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
using MayhemWpf.UserControls;
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
using MayhemCore;
using System.Threading;
using System.IO;
namespace VisionModules.Wpf
{

    /// <summary>
    /// Interaction logic for WebcamSnapshotConfig.xaml
    /// </summary>
    public partial class PictureConfig : WpfConfiguration
    {
        public string SaveLocation
        {
            get;
            private set; 
        }

        public string FilenamePrefix
        {
            get;
            private set;
        }

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

        public PictureConfig(string location, string prefix,  double capture_offset_time)
        {
            this.SaveLocation = location;
            slider_value = capture_offset_time;
            FilenamePrefix = prefix; 

            InitializeComponent();
        }

        public override void OnLoad()
        {
            // populate device list

            box_current_loc.Text = SaveLocation;

            Logger.WriteLine("Nr of Cameras available: " + i.DeviceCount);

            foreach (Camera c in i.cameras_available)
            {
                deviceList.Items.Add(c);
            }

            deviceList.SelectedIndex = 0;

            if (i.DeviceCount > 0)
            {
                // attach canvases with camera images to camera_preview_panel
                foreach (Camera c in i.cameras_available)
                {

                    cams.Add(c);

                    if (!c.running)
                    {
                        c.OnImageUpdated += i_OnImageUpdated;
                        ThreadPool.QueueUserWorkItem(new WaitCallback((o) =>
                        {
                            // Response timeout for the camera
                            //Thread.Sleep(350);
                            c.StartFrameGrabbing();
                        }));
                        Logger.WriteLine("using " + c.Info.ToString());
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
            //

            // directory and prefix boxes
            box_current_loc.Text = SaveLocation;
            box_filename_prefix.Text = FilenamePrefix; 
           
            CanSave = true;
        }

        public override void OnClosing()
        {
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
            Logger.WriteLine("New Image on Camera " + cam.index + " : " + cam.Info);

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
        /// Shows the dialog to select the file name of the template image
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            FolderBrowserDialog dlg = new FolderBrowserDialog();
            dlg.RootFolder = Environment.SpecialFolder.MyComputer;

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                SaveLocation = dlg.SelectedPath;
                box_current_loc.Text = SaveLocation;
            }
        }

        public override void OnCancel()
        {
            OnClosing();
        }

        public override void OnSave()
        {
            selected_camera = deviceList.SelectedItem as Camera;

            OnClosing();
        }

        public override string Title
        {
            get
            {
                return "Picture";
            }
        }

        private void CheckValidity()
        {
            CanSave = true;
            if (!(box_current_loc.Text.Length > 0 && Directory.Exists(box_current_loc.Text)))
            {
                textInvalid.Text = "Invalid save location";
                CanSave = false;
            }
            else if (box_current_loc.Text.Length == 0)
            {
                textInvalid.Text = "You must enter a filename prefix";
                CanSave = false;
            }
            else if (box_current_loc.Text.Length > 20)
            {
                textInvalid.Text = "Filename prefix is too long";
                CanSave = false;
            }
            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }

        public override void OnSave()
        {
            SaveLocation = box_current_loc.Text;
            FilenamePrefix = box_filename_prefix.Text;
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

        private void lbl_current_loc_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();
            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }

        private void box_filename_prefix_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckValidity();
            textInvalid.Visibility = CanSave ? Visibility.Collapsed : Visibility.Visible;
        }
    }
}
