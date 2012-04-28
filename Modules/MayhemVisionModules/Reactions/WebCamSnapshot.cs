using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemWebCamWrapper;
using MayhemVisionModules.Wpf;
using MayhemWpf.UserControls;
using MayhemWpf.ModuleTypes;
using MayhemVisionModules.Components;
using System.Windows.Forms;
using System.Media;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.Runtime.CompilerServices;
using MadWizard.WinUSBNet;
using System.Windows.Threading;
using System.Threading;


namespace MayhemVisionModules.Reactions
{
    [DataContract]
    [MayhemModule("Webcam Snapshot", "Save a snapshot from a webcam")] 
    public class WebCamSnapshot : ReactionBase, IWpfConfigurable
    {
        [DataMember]
        private string selectedCameraPath; //this is the selected camera

        [DataMember]
        private string folderLocation;

        [DataMember]
        private string fileNamePrefix;

        [DataMember]
        private string selectedCameraName;

        [DataMember]
        private int selectedCameraIndex;

        [DataMember]
        private int captureWidth;

        [DataMember]
        private int captureHeight;

        [DataMember]
        private bool showPreview;

        [DataMember]
        private bool playShutterSound;

        [DataMember]
        private bool selectedCameraConnected;

        private WebCamBuffer webcambuffer; //this registers to obtain images from the configured webcam

        ~WebCamSnapshot()
        {
            if (selectedCameraIndex != -1 && selectedCameraConnected)
                webcambuffer.UnregisterForImages(WebcamManager.GetCamera(selectedCameraIndex));
            WebcamManager.ReleaseInactiveCameras();
            WebcamManager.UnregisterWebcamConnectionEvent(OnCameraConnected);
            WebcamManager.UnregisterWebcamConnectionEvent(OnCameraDisconnected);
        }

        protected int LookforSelectedCamera()
        {
            bool selectedCameraFound = false;
            selectedCameraConnected = false;
            int numberConnectedCameras = WebcamManager.NumberConnectedCameras();
            if (numberConnectedCameras == 0)
            {
                selectedCameraConnected = false;
                Logger.WriteLine("No camera available");
                ErrorLog.AddError(ErrorType.Failure, "Webcam snapshot is disabled because no camera was detected");
                return -1;
            }
            int index = -1;
            for (int i = 0; i < numberConnectedCameras; i++)
            {
                if (WebcamManager.GetCamera(i).WebCamPath == selectedCameraPath)
                {
                    if (WebcamManager.GetCamera(i).IsActive)
                    {
                        index = i;
                        selectedCameraConnected = true;
                        selectedCameraFound = true;
                        break;
                        
                    }
                    else if (WebcamManager.StartCamera(i, captureWidth, captureHeight))
                    {
                        index = i;
                        selectedCameraConnected = true;
                        selectedCameraFound = true;
                        break;
                    }
                }
            }

            if (!selectedCameraFound && numberConnectedCameras > 0)
            {
                ErrorLog.AddError(ErrorType.Failure, "The originally selected camera is not available.");
            }
            return index;
        }

        protected void ReleasePreviousBuffers()
        {
            for (int i = 0; i < webcambuffer.SubscribedImagers.Count; i++)
                webcambuffer.UnregisterForImages(webcambuffer.SubscribedImagers[i]);
            webcambuffer.ClearBuffer();
        }

        public void OnCameraConnected(object sender, USBEvent e)
        {
            if (!selectedCameraConnected)
            {
                Thread.Sleep(50);
                WebcamManager.RestartService();
                selectedCameraIndex = LookforSelectedCamera();
                if (selectedCameraIndex != -1)
                {
                    Logger.WriteLine("Selected camera reconnected");
                    ErrorLog.AddError(ErrorType.Message, "Selected camera has been reconnected");
                    webcambuffer.RegisterForImages(WebcamManager.GetCamera(selectedCameraIndex));
                    WebcamManager.ReleaseInactiveCameras();
                }

            }
        }

        public void OnCameraDisconnected(object sender, USBEvent e)
        {
            string[] changedParts = WebcamManager.GetDeviceInfoFromPath(e.DevicePath);
            string[] selectedParts = WebcamManager.GetDeviceInfoFromPath(selectedCameraPath);
            if (string.Compare(changedParts[1], selectedParts[1], true) == 0 && string.Compare(changedParts[2], selectedParts[2], true) == 0)
            {
                selectedCameraConnected = false;
                ReleasePreviousBuffers();
                WebcamManager.ReleaseInactiveCameras();
                Logger.WriteLine("Selected camera disconnected");
                ErrorLog.AddError(ErrorType.Warning, "Selected camera has been disconnected");
            }
        }

       
       
        protected override void OnLoadDefaults()
        {
            webcambuffer = new WebCamBuffer();
            folderLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            fileNamePrefix = "Mayhem";
            captureWidth = 640;
            captureHeight = 480;
            showPreview = true;
            playShutterSound = true;
       
            if (IsEnabled)
            {
                if (WebcamManager.IsServiceRestartRequired())
                    WebcamManager.RestartService();
                selectedCameraIndex = -1;
                int numberConnectedCameras = WebcamManager.NumberConnectedCameras();
                if (numberConnectedCameras == 0)
                {
                    selectedCameraConnected = false;
                    Logger.WriteLine("No camera available");
                    ErrorLog.AddError(ErrorType.Warning, "Webcam snapshot is disabled because no camera was detected");
                }
                for (int i = 0; i < numberConnectedCameras; i++)
                {
                    if (WebcamManager.GetCamera(i).IsActive)
                    {
                        selectedCameraConnected = true;
                        selectedCameraPath = WebcamManager.GetCamera(i).WebCamPath;
                        selectedCameraName = WebcamManager.GetCamera(i).WebCamName;
                        selectedCameraIndex = i;
                        break;
                    }
                    else if (WebcamManager.StartCamera(i, captureWidth, captureHeight)) 
                    {
                        selectedCameraConnected = true;
                        selectedCameraPath = WebcamManager.GetCamera(i).WebCamPath;
                        selectedCameraName = WebcamManager.GetCamera(i).WebCamName;
                        selectedCameraIndex = i;
                        break;
                    }
                }
                if (selectedCameraIndex != -1)
                {
                    selectedCameraConnected = true;
                    if (webcambuffer != null)
                        ReleasePreviousBuffers();
                    webcambuffer.RegisterForImages(WebcamManager.GetCamera(selectedCameraIndex));
                }
            }
        }

        protected override void OnLoadFromSaved()
        {
            webcambuffer = new WebCamBuffer();

            if (IsEnabled)
            {
                if (WebcamManager.IsServiceRestartRequired())
                    WebcamManager.RestartService();

                //look for the selected camera
                selectedCameraIndex = LookforSelectedCamera();
                if (selectedCameraIndex != -1 && selectedCameraConnected)
                {
                    if (webcambuffer != null)
                        ReleasePreviousBuffers();
                    webcambuffer.RegisterForImages(WebcamManager.GetCamera(selectedCameraIndex));
                }
            }
        }

        protected override void OnAfterLoad()
        {
            if (IsEnabled)
            {
                WebcamManager.RegisterWebcamConnectionEvent(OnCameraConnected);
                WebcamManager.RegisterWebcamRemovalEvent(OnCameraDisconnected);
            }
            WebcamManager.ReleaseInactiveCameras();
           
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected override void OnDisabled(DisabledEventArgs e)
        {
            if (!e.IsConfiguring)
            {
                if (selectedCameraIndex != -1 && webcambuffer != null)
                        ReleasePreviousBuffers();
                WebcamManager.ReleaseInactiveCameras();
                WebcamManager.UnregisterWebcamConnectionEvent(OnCameraConnected);
                WebcamManager.UnregisterWebcamConnectionEvent(OnCameraDisconnected);
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected override void OnEnabling(EnablingEventArgs e)
        {
            if (!e.WasConfiguring)
            {
                if (WebcamManager.IsServiceRestartRequired())
                    WebcamManager.RestartService();
                //look for the selected camera
                selectedCameraIndex = LookforSelectedCamera();
                if (selectedCameraIndex != -1 && selectedCameraConnected)
                {
                    WebcamManager.RegisterWebcamConnectionEvent(OnCameraConnected);
                    WebcamManager.RegisterWebcamRemovalEvent(OnCameraDisconnected);
                    if (webcambuffer != null)
                        ReleasePreviousBuffers();
                    ReleasePreviousBuffers();
                    webcambuffer.RegisterForImages(WebcamManager.GetCamera(selectedCameraIndex));
                }
                WebcamManager.ReleaseInactiveCameras();
            }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            WebcamSnapshotConfig config = configurationControl as WebcamSnapshotConfig;
            webcambuffer = new WebCamBuffer();
            if (WebcamManager.IsServiceRestartRequired())
                WebcamManager.RestartService();
            WebcamManager.RegisterWebcamConnectionEvent(OnCameraConnected);
            WebcamManager.RegisterWebcamRemovalEvent(OnCameraDisconnected);
            folderLocation = config.SaveLocation;
            fileNamePrefix = config.FilenamePrefix;
            captureWidth = 640;
            captureHeight = 480;
            selectedCameraPath = config.SelectedCameraPath;
            selectedCameraName = config.SelectedCameraName;
            showPreview = config.ShowPreview;
            playShutterSound = config.PlayShutterSound;
        }

        protected override void OnDeleted()
        {
            if (selectedCameraIndex != -1)
                webcambuffer.UnregisterForImages(WebcamManager.GetCamera(selectedCameraIndex));
            WebcamManager.ReleaseInactiveCameras();
            WebcamManager.UnregisterWebcamConnectionEvent(OnCameraConnected);
            WebcamManager.UnregisterWebcamConnectionEvent(OnCameraDisconnected);
        }

        public override void Perform()
        {
            if (selectedCameraIndex != -1 && selectedCameraConnected)
            {
                Bitmap image = webcambuffer.GetLastBufferedItem();
                if (image != null)
                {
                    string savedPath = SaveImage(image);
                    //save image
                    if (playShutterSound)
                    {
                        SoundPlayer shutterSound = new SoundPlayer("C:\\Program Files (x86)\\Outercurve\\Mayhem\\VisionModules\\lib\\camera1.wav");
                        shutterSound.Play();
                    }

                    if (showPreview)
                    {
                        //   BitmapImage preview = new BitmapImage(new Uri(savedPath));

                    }
                }
            }
            else
            {
                ReleasePreviousBuffers();
                WebcamManager.ReleaseInactiveCameras();
                ErrorLog.AddError(ErrorType.Failure, "Webcam snapshot is disabled because selected camera was not found");
            }
        }

        public string SaveImage(Bitmap image)
        {
            Logger.WriteLine("SaveImage");
            DateTime now = DateTime.Now;
            string filename = fileNamePrefix + "_" +
                                now.Year.ToString("D2") + "-" +
                                now.Month.ToString("D2") + "-" +
                                now.Day.ToString("D2") + "_" +
                                now.Hour.ToString("D2") + "-" +
                                now.Minute.ToString("D2") + "-" +
                                now.Second.ToString("D2") + ".jpg";
            string path = folderLocation + "\\" + filename;
            Logger.WriteLine("saving file to " + path);
            try
            {
                image.Save(path, ImageFormat.Jpeg);
                ErrorLog.AddError(ErrorType.Message, "Picture saved to: " + path);
            }
            catch
            {
                Logger.WriteLine("Exception while saving picture");
                ErrorLog.AddError(ErrorType.Failure, "Could not save a picture to: " + path);
            }
            finally
            {
                // VERY important! 
                image.Dispose();
            }
            return path; 
        }

        public WpfConfiguration ConfigurationControl
        {
            get
            {
                ReleasePreviousBuffers();
                WebcamManager.ReleaseInactiveCameras();
                WebcamSnapshotConfig config = new WebcamSnapshotConfig(folderLocation, fileNamePrefix,showPreview, playShutterSound);
                return config;
            }
        }

       
        public string GetConfigString()
        {
            return string.Format("\"{0}\" ", selectedCameraName);
        }
    }
}
