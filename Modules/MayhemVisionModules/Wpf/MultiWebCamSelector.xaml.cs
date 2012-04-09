
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
            previewWidth = 320; //the size of the preview windows
            previewHeight = 240;
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
            for (int i = 0; i < numberConnectedCameras; i++)
            {
                if (WebcamManager.StartCamera(i, captureWidth, captureHeight))
                {
                    ImageViewer viewer = new ImageViewer();
                    viewer.ViewerWidth = previewWidth;
                    viewer.ViewerHeight = previewHeight;
                    viewer.ViewerTranslateX = 0.25*previewWidth;
                    viewer.TopTitle = WebcamManager.GetCamera(i).WebCamName;
                    viewer.SetImageSource(WebcamManager.GetCamera(i));
                    cameraPreviews.Add(viewer);
                    ++numAvailableCameras;
                }
            }
            ImageViewer selectedView = new ImageViewer();
            selectedView.ViewerWidth = captureWidth;
            selectedView.ViewerHeight = captureHeight;
            selectedView.TopTitle = WebcamManager.GetCamera(0).WebCamName;
            selectedView.SetImageSource(WebcamManager.GetCamera(0));
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
    }
}