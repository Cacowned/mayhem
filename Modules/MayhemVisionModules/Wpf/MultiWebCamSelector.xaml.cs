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



namespace MayhemVisionModules.Wpf
{
    /// <summary>
    /// Interaction logic for MultiWebCamSelector.xaml
    /// </summary>
    public partial class MultiWebCamSelector : UserControl
    {
        public MultiWebCamSelector()
        {
            InitializeComponent();
            Dispatcher.ShutdownStarted += OnExit;
            captureWidth = 640;  //the size of the capture...
            captureHeight = 480;
            previewWidth = 160; //the size of the preview windows
            previewHeight = 120;
            mainViewWidth = 640;
            mainViewHeight = 480;
            //refresh the camera list on construction...
            WebcamManager.CleanUp();
            RefreshCameraList();
            StartHardwareScan();
        }

        public void RefreshCameraList()
        {
            WebcamManager.UpdateCameraList();
            numberConnectedCameras = WebcamManager.NumberConnectedCameras();
            cameraPreviews = new ObservableCollection<ImageViewer>();
            mainView = new ObservableCollection<ImageViewer>();
            selectedCameraIndex = -1;
            int numAvailableCameras = 0;
            //populate the camera list and find the one that is available and set it as current selection...
            selectedCameraIndex = 0;
            for (int i = 0; i < numberConnectedCameras; i++)
            {
                if (WebcamManager.StartCamera(i, captureWidth, captureHeight))
                {
                    ImageViewer viewer = new ImageViewer();
                    viewer.ViewerWidth = previewWidth;
                    viewer.ViewerHeight = previewHeight;
                    viewer.TopTitle = WebcamManager.GetCamera(i).WebCamName;
                    if (viewer.TopTitle == selectedCameraName)
                        selectedCameraIndex = i;
                    viewer.MouseEnter += Viewer_MouseEnter;
                    viewer.MouseLeave += Viewer_MouseLeave;
                    viewer.PreviewMouseLeftButtonUp += Viewer_MouseLeftButtonUp;
                    viewer.SetImageSource(WebcamManager.GetCamera(i));
                    cameraPreviews.Add(viewer);
                    ++numAvailableCameras;
                }
            }
            ImageViewer selectedView = new ImageViewer();
            selectedView.ViewerWidth = mainViewWidth;
            selectedView.ViewerHeight = mainViewHeight;
            selectedView.ViewerBorderColor = "White";
            selectedView.TopTitle = WebcamManager.GetCamera(selectedCameraIndex).WebCamName;
            selectedView.SetImageSource(WebcamManager.GetCamera(selectedCameraIndex));
            mainView.Add(selectedView);

            camera_selection_panel.ItemsSource = cameraPreviews;
            main_view.ItemsSource = mainView;
            
            //release inactive cameras so that they can be made available to other processes...
            WebcamManager.ReleaseInactiveCameras();

           
        }

        private delegate void OneParameterDelegate();

        public void StartHardwareScan()
        {
            
            ScanThread = new Thread(ScanforHardwareChange);
            ScanThread.SetApartmentState(ApartmentState.STA);
            ScanThread.Start();
        }

        private void ScanforHardwareChange()
        {
            while (!stopScanThread)
            {
                Thread.Sleep(1);
                if (DllImport.IsAnyCameraConnectedOrDisconnected())
                {
                    CloseAll();
                    Dispatcher.BeginInvoke(new OneParameterDelegate(RefreshCameraList), null);
                }
            }
        }

        public void CloseAll()
        {
            WebcamManager.CleanUp();
        }

        private void Viewer_MouseEnter(object sender, MouseEventArgs e)
        {
            ImageViewer viewer = sender as ImageViewer;
            viewer.ViewerBorderColor = "LightGreen";
            viewer.ViewerWidth = previewHeight+16;
            viewer.ViewerHeight = viewer.ViewerWidth*3/4;
        }

        private void Viewer_MouseLeave(object sender, MouseEventArgs e)
        {
            ImageViewer viewer = sender as ImageViewer;
            viewer.ViewerBorderColor = "Beige";
            viewer.ViewerWidth = previewWidth;
            viewer.ViewerHeight = previewHeight;
           
        }

 
        private void Viewer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ImageViewer viewer = sender as ImageViewer;
            selectedCameraIndex = cameraPreviews.IndexOf(viewer);
            mainView[0].TopTitle = WebcamManager.GetCamera(selectedCameraIndex).WebCamName;
            mainView[0].SetImageSource(WebcamManager.GetCamera(selectedCameraIndex));
            selectedCameraName = mainView[0].TopTitle;
            
        }
 

        private void OnExit(object sender, EventArgs e)
        {
            stopScanThread = true;
            ScanThread.Join();
            CloseAll();
        }

        private volatile bool stopScanThread;
        private Thread ScanThread;
        public ObservableCollection<ImageViewer> mainView;
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

        private void Selector_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            previewWidth = Math.Max(Convert.ToInt16(ActualWidth) / 8, 160);
            previewHeight = previewWidth * 3 / 4;

            foreach (ImageViewer viewer in cameraPreviews)
            {
                viewer.ViewerWidth = previewWidth;
                viewer.ViewerHeight = previewHeight;
            }


            mainViewWidth = Math.Max(Convert.ToInt16(ActualWidth)/2, 320);
            mainViewHeight = mainViewWidth * 3 / 4;

            mainView[0].ViewerWidth = mainViewWidth;
            mainView[0].ViewerHeight = mainViewHeight;

            main_view.MaxHeight = mainViewHeight + 120;
            main_view.MaxWidth = mainViewWidth + 120;

            camera_selection_panel.MaxHeight = previewHeight+60;
            camera_selection_panel.MaxWidth = ActualWidth;
        }
     }
}