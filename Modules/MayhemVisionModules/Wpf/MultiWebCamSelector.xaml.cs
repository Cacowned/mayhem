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
  
        }
    
        public void RefreshCameraList()
        {
            WebcamManager.UpdateCameraList();
            numberConnectedCameras = WebcamManager.NumberConnectedCameras();
            cameraPreviews = new ObservableCollection<ImageViewer>();
            cameras = new List<WebCam>();
            selectedCameraIndex = -1;
            int numAvailableCameras = 0;
            //populate the camera list and find the one that is available and set it as current selection...
            for (int i = 0; i < numberConnectedCameras; i++)
            {
                cameras.Add(WebcamManager.GetCamera(i));
                cameras[i].Width = captureWidth;
                cameras[i].Height = captureHeight;
                try
                {
                    cameras[i].Start();
                }
                catch (Exception)
                {
                    cameras[i].Stop();
                    continue;
                }
                finally
                {
                }
                ImageViewer viewer = new ImageViewer();
                viewer.AddImageSource(cameras[i]);
                viewer.ViewerWidth = previewWidth;
                viewer.ViewerHeight = previewHeight;
                //viewer.Title.Text = cameras[i].WebCamName;
                cameraPreviews.Add(viewer);
                ++numAvailableCameras;
            }
            camera_selection_panel.Width = previewWidth * numAvailableCameras;
            camera_selection_panel.Height = previewHeight;
            camera_selection_panel.ItemsSource = cameraPreviews;
            for (int i = 0; i < cameras.Count; i++)
            {
                if (cameras[i].Subsribers.Count == 0)
                {
                    cameras[i].Stop();
                    DllImport.StopWebCam(cameras[i].WebCamID);
                }
            }
        }


        public void CloseAll()
        {
            foreach (WebCam cam in cameras)
            {
                cam.Stop();

            }
            WebcamManager.CleanUp();
        }

        private void OnExit(object sender, EventArgs e)
        {
            CloseAll();
        }

        public ObservableCollection<ImageViewer> cameraPreviews; //the WebCam class is implemented as an image source!
        public List<WebCam> cameras;
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
