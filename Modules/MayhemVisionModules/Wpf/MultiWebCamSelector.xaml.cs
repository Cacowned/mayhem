﻿
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
using System.Drawing;
using System.Collections.ObjectModel;
using MayhemWebCamWrapper;
using MayhemVisionModules.Wpf;
using System.IO;
using System.Threading;
using System.Windows.Threading;
using MadWizard.WinUSBNet;


namespace MayhemVisionModules.Wpf
{

   
    /// <summary>
    /// Interaction logic for MultiWebCamSelector.xaml
    /// </summary>
    public partial class MultiWebCamSelector : UserControl, IDisposable
    {

        [System.Runtime.InteropServices.DllImport("gdi32")]
        static extern int DeleteObject(IntPtr o);

        public static BitmapSource loadBitmap(System.Drawing.Bitmap source)
        {
            IntPtr ip = source.GetHbitmap();
            BitmapSource bs = null;
            try
            {
                bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip,
                   IntPtr.Zero, Int32Rect.Empty,
                   System.Windows.Media.Imaging.BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(ip);
            }

            return bs;
        } 


        public MultiWebCamSelector()
        {
            InitializeComponent();
            Dispatcher.ShutdownStarted += OnExit;

            captureWidth = 320;  //the size of the capture...
            captureHeight = 240;
            previewWidth = 80; //the size of the preview windows
            previewHeight = 60;
            mainViewWidth = 320;
            mainViewHeight = 240;
            //refresh the camera list on construction...
        }

        public void InitSelector()
        {
            captureWidth = 320;  //the size of the capture...
            captureHeight = 240;
            previewWidth = 80; //the size of the preview windows
            previewHeight = 60;
            mainViewWidth = 320;
            mainViewHeight = 240;
            //refresh the camera list on construction...
            RefreshCameraList();
            WebcamManager.RegisterWebcamConnectionEvent(OnCameraConnected);
            WebcamManager.RegisterWebcamRemovalEvent(OnCameraDisconnected);
        }



        public void OnCameraConnected(object sender, USBEvent e)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, (SendOrPostCallback)delegate
            {
                Thread.Sleep(30);
                RefreshCameraList();
            }, null);
        }

        public void OnCameraDisconnected(object sender, USBEvent e)
        {
            Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Render, (SendOrPostCallback)delegate
            {
                RefreshCameraList();
            }, null);
        }


        public void RefreshCameraList()
        {
            if (WebcamManager.IsServiceRestartRequired())
                WebcamManager.RestartService();
            numberConnectedCameras = WebcamManager.NumberConnectedCameras();
            cameraPreviews = new ObservableCollection<ImageViewer>();
            //mainView = new ObservableCollection<ImageViewer>();
            cameraList = new List<int>();
            selectedCameraIndex = -1;
            int numAvailableCameras = 0;
            //populate the camera list and find the one that is available and set it as current selection...
            for (int i = 0; i < numberConnectedCameras; i++)
            {
                //if (WebcamManager.StartCamera(i, captureWidth, captureHeight))
                {
                    ImageViewer viewer = new ImageViewer();
                    viewer.ViewerWidth = previewWidth;
                    viewer.ViewerHeight = previewHeight;
                    viewer.TopTitle = WebcamManager.GetCamera(i).WebCamName;
                    
                    if (WebcamManager.StartCamera(i, captureWidth, captureHeight))
                    {
                        if (viewer.TopTitle == selectedCameraName || selectedCameraIndex == -1)
                        {
                            selectedCameraIndex = i;
                        }
                        viewer.SetImageSource(WebcamManager.GetCamera(i));
                        viewer.MouseEnter += Viewer_MouseEnter;
                        viewer.MouseLeave += Viewer_MouseLeave;
                        viewer.PreviewMouseLeftButtonUp += Viewer_MouseLeftButtonUp;
                        ++numAvailableCameras;
                    }
                    else
                    {
                        viewer.ImageSource.Source = loadBitmap(Properties.Resources.CamBusyIcon);
                    }
                    cameraList.Add(i);
                    cameraPreviews.Add(viewer);
                }
            }

            if (selectedCameraIndex != -1)
            {
                cameraPreviews[cameraList.IndexOf(selectedCameraIndex)].ViewerBorderColor = "LightGreen";
                cameraPreviews[cameraList.IndexOf(selectedCameraIndex)].ViewerWidth = previewHeight + 16;
                cameraPreviews[cameraList.IndexOf(selectedCameraIndex)].ViewerHeight = cameraPreviews[cameraList.IndexOf(selectedCameraIndex)].ViewerWidth * 3 / 4;
            }
            //ImageViewer selectedView = new ImageViewer();
            //selectedView.ViewerWidth = mainViewWidth;
            //selectedView.ViewerHeight = mainViewHeight;
            //selectedView.ViewerBorderColor = "Transparent";
            //selectedView.TopTitle = WebcamManager.GetCamera(selectedCameraIndex).WebCamName;
            //selectedView.SetImageSource(WebcamManager.GetCamera(selectedCameraIndex));
            ////mainView.Add(selectedView);

            camera_selection_panel.ItemsSource = cameraPreviews;
            //main_view.ItemsSource = mainView;

            
            //release inactive cameras so that they can be made available to other processes...
            WebcamManager.ReleaseInactiveCameras();
        }

        private delegate void OneParameterDelegate();

       
        private void Viewer_MouseEnter(object sender, MouseEventArgs e)
        {
            ImageViewer mouseViewer = sender as ImageViewer;
            int index = cameraPreviews.IndexOf(mouseViewer);
            if (index != selectedCameraIndex && selectedCameraIndex != -1)
            {
                mouseViewer.ViewerWidth = previewWidth;
                mouseViewer.ViewerHeight = previewHeight;
            }
        }

        private void Viewer_MouseLeave(object sender, MouseEventArgs e)
        {
            ImageViewer mouseViewer = sender as ImageViewer;
            int index = cameraPreviews.IndexOf(mouseViewer);
            if (index != selectedCameraIndex && selectedCameraIndex != -1)
            {
                mouseViewer.ViewerWidth = previewWidth;
                mouseViewer.ViewerHeight = previewHeight;
            }
        }

 
        private void Viewer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ImageViewer mouseViewer = sender as ImageViewer;
            int index = cameraList[cameraPreviews.IndexOf(mouseViewer)];
            if (index != selectedCameraIndex && selectedCameraIndex != -1)
            {
                cameraPreviews[cameraList.IndexOf(selectedCameraIndex)].ViewerBorderColor = "Transparent";
                cameraPreviews[cameraList.IndexOf(selectedCameraIndex)].ViewerWidth = previewWidth;
                cameraPreviews[cameraList.IndexOf(selectedCameraIndex)].ViewerHeight = previewHeight;

                mouseViewer.ViewerBorderColor = "LightGreen";
                mouseViewer.ViewerWidth = previewHeight + 16;
                mouseViewer.ViewerHeight = mouseViewer.ViewerWidth * 3 / 4;
                selectedCameraIndex = cameraList[cameraPreviews.IndexOf(mouseViewer)];

                //mainView[0].TopTitle = WebcamManager.GetCamera(selectedCameraIndex).WebCamName;
                //mainView[0].SetImageSource(WebcamManager.GetCamera(selectedCameraIndex));
                selectedCameraName = WebcamManager.GetCamera(selectedCameraIndex).WebCamName;
            }
        }

        //This returns the unique identifier for the connected camera
        public string GetSelectedCameraPath()
        {
            if (selectedCameraIndex != -1)
                return WebcamManager.GetCamera(cameraList[selectedCameraIndex]).WebCamPath;
            else
               return null; 
        }

        public string GetSelectedCameraName()
        {
            if (selectedCameraIndex != -1)
                return WebcamManager.GetCamera(cameraList[selectedCameraIndex]).WebCamName;
            else
                return null;
        }

        public int GetSelectedCameraIndex()
        {
            return cameraList[selectedCameraIndex];
        }

        public void Cleanup()
        {
            foreach (ImageViewer viewer in cameraPreviews)
            {
                for (int i = 0; i < viewer.ImageSource.SubscribedImagers.Count; i++)
                    viewer.ImageSource.UnregisterForImages(viewer.ImageSource.SubscribedImagers[i]);
            }
            //foreach (ImageViewer viewer in mainView)
            //{
            //    for (int i = 0; i < viewer.ImageSource.SubscribedImagers.Count; i++)
            //        viewer.ImageSource.UnregisterForImages(viewer.ImageSource.SubscribedImagers[i]);
            //}
            WebcamManager.ReleaseInactiveCameras();
            WebcamManager.UnregisterWebcamConnectionEvent(OnCameraConnected);
            WebcamManager.UnregisterWebcamRemovalEvent(OnCameraDisconnected);
        }

        private void OnExit(object sender, EventArgs e)
        {
            Cleanup();
        }

        public void Dispose()
        {
            Cleanup();
        }

        //public ObservableCollection<ImageViewer> mainView;
        public ObservableCollection<ImageViewer> cameraPreviews; //the WebCam class is implemented as an image source!
        private int selectedCameraIndex;// the camera currently selected
        private string selectedCameraName;
        private int numberConnectedCameras; //the number of cameras detected (includes those that are busy with other appplications as well)
        //dimensions of the capture
        private int captureWidth;
        private int captureHeight;
        //dimensions of the previews
        private int previewWidth;
        private int previewHeight;

        //dimensions of the main camera view
        private int mainViewWidth;
        private int mainViewHeight;
        //map associating panel with camera indices
        private List<int> cameraList;
        private void Selector_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            previewWidth = Math.Max(Convert.ToInt16(ActualWidth) / 8, 160);
            previewHeight = previewWidth * 3 / 4;

            foreach (ImageViewer viewer in cameraPreviews)
            {
                viewer.ViewerWidth = previewWidth;
                viewer.ViewerHeight = previewHeight;
            }


            //mainViewWidth = Math.Max(Convert.ToInt16(ActualWidth)/2, 320);
            //mainViewHeight = mainViewWidth * 3 / 4;

            //mainView[0].ViewerWidth = mainViewWidth;
            //mainView[0].ViewerHeight = mainViewHeight;

            //main_view.MaxHeight = mainViewHeight + 120;
            //main_view.MaxWidth = mainViewWidth + 120;

            camera_selection_panel.MaxHeight = previewHeight+60;
            camera_selection_panel.MaxWidth = ActualWidth;
        }
     }
}