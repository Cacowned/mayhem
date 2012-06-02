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
using System.IO;


namespace MayhemVisionModules.Reactions
{
    [DataContract]
    [MayhemModule("Webcam Snapshot", "Saves a snapshot from a webcam")] 
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

        private WebCamBuffer webcambuffer=null; //this registers to obtain images from the configured webcam

        private bool isPerforming = false;
        private bool callbacksRegistered = false;

        ~WebCamSnapshot()
        {
            if (selectedCameraIndex != -1 && selectedCameraConnected && webcambuffer != null)
                webcambuffer.UnregisterForImages(WebcamManager.GetCamera(selectedCameraIndex));
            WebcamManager.ReleaseInactiveCameras();
            if (callbacksRegistered)
            {
                WebcamManager.UnregisterWebcamConnectionEvent(OnCameraConnected);
                WebcamManager.UnregisterWebcamRemovalEvent(OnCameraDisconnected);
                callbacksRegistered = false;
            }
        }

        protected int LookforSelectedCamera(bool force = false)
        {
            bool selectedCameraFound = false;
            selectedCameraConnected = false;
            if (!IsEnabled && !force)
                return -1;
            int numberConnectedCameras = WebcamManager.NumberConnectedCameras();
            if (numberConnectedCameras == 0)
            {
                selectedCameraConnected = false;
                Logger.WriteLine("No camera available");
                ErrorLog.AddError(ErrorType.Failure, "Motion detection is disabled because no camera was detected");
                return -1;
            }
            int index = -1;
            if (selectedCameraPath == default(string))
            {
                ErrorLog.AddError(ErrorType.Message, "No webcam configuration. Defaulting to first available webcam.");
                return -1;
            }
            else
            {
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
        }

        protected void ReleasePreviousBuffers()
        {
            if (webcambuffer != null && selectedCameraIndex != -1)
            {
                for (int i = 0; i < webcambuffer.SubscribedImagers.Count; i++)
                    webcambuffer.UnregisterForImages(webcambuffer.SubscribedImagers[i]);
                webcambuffer.ClearBuffer();
                webcambuffer = null;
            }
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
       
        }

        protected override void OnLoadFromSaved()
        {
            webcambuffer = new WebCamBuffer();
            captureWidth = 640;
            captureHeight = 480;
        }

        void InitializeSnapshot(int cameraindex)
        {
            Thread thread = new Thread(() =>
            {
                try
                {
                    ReleasePreviousBuffers();
                    webcambuffer = new WebCamBuffer();
                    webcambuffer.RegisterForImages(WebcamManager.GetCamera(cameraindex));
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.ToString());
                }
                Thread.Sleep(30);
                WebcamManager.ReleaseInactiveCameras();
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }


        protected override void OnAfterLoad()
        {
            selectedCameraIndex = LookforSelectedCamera();
            if (selectedCameraIndex != -1 && selectedCameraConnected)
            {
                if (WebcamManager.IsServiceRestartRequired())
                    WebcamManager.RestartService();
                if (!callbacksRegistered)
                {
                    WebcamManager.RegisterWebcamConnectionEvent(OnCameraConnected);
                    WebcamManager.RegisterWebcamRemovalEvent(OnCameraDisconnected);
                    callbacksRegistered = true;
                }
                //look for the selected camera
                InitializeSnapshot(selectedCameraIndex);
            }
           
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected override void OnDisabled(DisabledEventArgs e)
        {
            //if (!e.IsConfiguring)
            {
                if (webcambuffer != null && selectedCameraIndex != -1)
                {
                    webcambuffer.UnregisterForImages(WebcamManager.GetCamera(selectedCameraIndex));
                    ReleasePreviousBuffers();
                    WebcamManager.ReleaseInactiveCameras();
                    if (callbacksRegistered)
                    {
                        WebcamManager.UnregisterWebcamConnectionEvent(OnCameraConnected);
                        WebcamManager.UnregisterWebcamRemovalEvent(OnCameraDisconnected);
                        callbacksRegistered = false;
                    }
                }

            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected override void OnEnabling(EnablingEventArgs e)
        {
            //if (!e.WasConfiguring)
            {
                if (WebcamManager.IsServiceRestartRequired())
                    WebcamManager.RestartService();
                //look for the selected camera
                selectedCameraIndex = LookforSelectedCamera(true);
                if (selectedCameraIndex != -1 && selectedCameraConnected)
                {
                    if (!callbacksRegistered)
                    {
                        WebcamManager.RegisterWebcamConnectionEvent(OnCameraConnected);
                        WebcamManager.RegisterWebcamRemovalEvent(OnCameraDisconnected);
                        callbacksRegistered = true;
                    }
                    InitializeSnapshot(selectedCameraIndex);
                }
            }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            WebcamSnapshotConfig config = configurationControl as WebcamSnapshotConfig;
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

            if (webcambuffer != null && selectedCameraIndex != -1)
            {
                webcambuffer.UnregisterForImages(WebcamManager.GetCamera(selectedCameraIndex));
                ReleasePreviousBuffers();
                WebcamManager.ReleaseInactiveCameras();
                if (callbacksRegistered)
                {
                    WebcamManager.UnregisterWebcamConnectionEvent(OnCameraConnected);
                    WebcamManager.UnregisterWebcamRemovalEvent(OnCameraDisconnected);
                    callbacksRegistered = false;
                }
            }
       }

        public override void Perform()
        {
            if (isPerforming)
            {
                ErrorLog.AddError(ErrorType.Failure, "Waiting for last snapshot to save");
                return;
            }
            if (!selectedCameraConnected)
            {
                ErrorLog.AddError(ErrorType.Failure, "Waiting for webcam to become available");
                return;
            }
            if (selectedCameraIndex != -1 && selectedCameraConnected && webcambuffer != null)
            {
                isPerforming = true;
                Bitmap image = webcambuffer.GetLastBufferedItem();
                try
                {
                    if (image != null)
                    {
                        string savedPath = SaveImage(ref image);
                        //save image
                        if (playShutterSound)
                        {
                            SoundPlayer shutterSound = new SoundPlayer(Properties.Resources.shutterSound);
                            shutterSound.Play();
                        }

                        if (showPreview)
                        {
                            ImagePopup popup = new ImagePopup();
                            popup.ShowPopup(1000, savedPath);
                            popup = null;
                        }

                    }
                }
                catch
                {
                    Logger.WriteLine("Exception while saving picture");
                    ErrorLog.AddError(ErrorType.Failure, "Could not save picture");
                }
                finally
                {
                    if (image != null)
                        image.Dispose();
                    image = null;
                }
                isPerforming = false;
            }
            else
            {
                ReleasePreviousBuffers();
                WebcamManager.ReleaseInactiveCameras();
                ErrorLog.AddError(ErrorType.Failure, "Webcam snapshot is disabled because selected camera was not found");
            }
        }

        public string SaveImage(ref Bitmap image)
        {
            Bitmap tempImage = image;
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
                tempImage.Save(path, ImageFormat.Jpeg);
                ErrorLog.AddError(ErrorType.Message, "Picture saved to: " + path);
            }
            catch
            {
                Logger.WriteLine("Exception while saving picture");
                ErrorLog.AddError(ErrorType.Failure, "Could not save a picture to: " + path);
            }
            tempImage.Dispose();
            tempImage = null;
            return path; 
        }

        public WpfConfiguration ConfigurationControl
        {
            get
            {
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
