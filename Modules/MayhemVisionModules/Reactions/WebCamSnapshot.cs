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

        private WebCamBuffer webcambuffer; //this registers to obtain images from the configured webcam

      

        ~WebCamSnapshot()
        {
            if (selectedCameraIndex != -1)
                webcambuffer.UnregisterForImages(WebcamManager.GetCamera(selectedCameraIndex));
            WebcamManager.ReleaseInactiveCameras();
        }

        protected int LookforSelectedCamera()
        {
            int numberConnectedCameras = WebcamManager.NumberConnectedCameras();
            if (numberConnectedCameras == 0)
            {
                Logger.WriteLine("No camera available");
                ErrorLog.AddError(ErrorType.Warning, "Webcam snapshot is disabled because no camera was detected");
                return -1;
            }
            bool selectedCameraFound = false;
            int index = -1;
            for (int i = 0; i < numberConnectedCameras; i++)
            {
                if (WebcamManager.GetCamera(i).WebCamPath == selectedCameraPath)
                {
                    if (WebcamManager.GetCamera(i).IsActive)
                    {
                        index = i;
                        selectedCameraFound = true;
                        break;
                        
                    }
                    else if (WebcamManager.StartCamera(i, captureWidth, captureHeight))
                    {
                        index = i;
                        selectedCameraFound = true;
                        break;
                    }
                }
            }

            if (!selectedCameraFound && numberConnectedCameras > 0)
            {
                ErrorLog.AddError(ErrorType.Warning, "The originally selected camera is busy. Defaulting to first camera. Please check your configuration");
                for (int i = 0; i < numberConnectedCameras; i++)
                {
                    if (WebcamManager.GetCamera(i).IsActive)
                    {
                        index = i;
                        break;
                    }
                    else if (WebcamManager.StartCamera(i, captureWidth, captureHeight))
                    {
                        index = i;
                        break;
                    }

                }
            }
            return index;
        }

        protected void ReleasePreviousBuffers()
        {
            for (int i = 0; i < webcambuffer.SubscribedImagers.Count; i++)
                webcambuffer.UnregisterForImages(webcambuffer.SubscribedImagers[i]);
        }

        protected override void OnLoadDefaults()
        {
            webcambuffer = new WebCamBuffer();
            if (IsEnabled)
            {
                WebcamManager.StartServiceIfNeeded();
                selectedCameraIndex = -1;
                folderLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
                fileNamePrefix = "Mayhem";
                captureWidth = 640;
                captureHeight = 480;
                showPreview = true;
                playShutterSound = true;
                int numberConnectedCameras = WebcamManager.NumberConnectedCameras();
                if (numberConnectedCameras == 0)
                {
                    Logger.WriteLine("No camera available");
                    ErrorLog.AddError(ErrorType.Warning, "Webcam snapshot is disabled because no camera was detected");
                }
                for (int i = 0; i < numberConnectedCameras; i++)
                {
                    if (WebcamManager.GetCamera(i).IsActive)
                    {
                        selectedCameraIndex = i;
                        break;
                    }
                    else if (WebcamManager.StartCamera(i, captureWidth, captureHeight)) 
                    {
                        selectedCameraIndex = i;
                        break;
                    }
                }
                if (selectedCameraIndex != -1 && webcambuffer != null)
                {
                    ReleasePreviousBuffers();
                    webcambuffer.RegisterForImages(WebcamManager.GetCamera(selectedCameraIndex));
                }
                //shutdown all other webcams...
                WebcamManager.ReleaseInactiveCameras();
            }
        }

        protected override void OnAfterLoad()
        {
            webcambuffer = new WebCamBuffer();
            if (IsEnabled)
            {
                WebcamManager.StartServiceIfNeeded();
                selectedCameraIndex = -1;
                //look for the selected camera
                selectedCameraIndex = LookforSelectedCamera();
                if (selectedCameraIndex != -1 && webcambuffer != null)
                {
                    ReleasePreviousBuffers();
                    webcambuffer.RegisterForImages(WebcamManager.GetCamera(selectedCameraIndex));
                }
                WebcamManager.ReleaseInactiveCameras();
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected override void OnDisabled(DisabledEventArgs e)
        {
            if (!e.IsConfiguring)
            {
                if (selectedCameraIndex != -1 && webcambuffer != null)
                    ReleasePreviousBuffers();
                WebcamManager.ReleaseInactiveCameras();
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected override void OnEnabling(EnablingEventArgs e)
        {
            if (!e.WasConfiguring)
            {
                WebcamManager.StartServiceIfNeeded();
                //look for the selected camera
                if (selectedCameraIndex != -1 && webcambuffer != null)
                {
                    ReleasePreviousBuffers();
                    webcambuffer.RegisterForImages(WebcamManager.GetCamera(selectedCameraIndex));
                }
                WebcamManager.ReleaseInactiveCameras();
            }
        }

        protected override void OnDeleted()
        {
            if (selectedCameraIndex != -1)
                webcambuffer.UnregisterForImages(WebcamManager.GetCamera(selectedCameraIndex));
            WebcamManager.ReleaseInactiveCameras();
        }

        public override void Perform()
        {
            int index = selectedCameraIndex;
            selectedCameraIndex = LookforSelectedCamera();
            if (index != -1)
            {
                if (index != selectedCameraIndex)
                {
                    ReleasePreviousBuffers();
                    webcambuffer.RegisterForImages(WebcamManager.GetCamera(selectedCameraIndex));
                    WebcamManager.ReleaseInactiveCameras();
                }
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
                WebcamSnapshotConfig config = new WebcamSnapshotConfig(folderLocation, fileNamePrefix, captureWidth, captureHeight, selectedCameraPath, showPreview, playShutterSound);
                return config;
            }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            WebcamSnapshotConfig config = configurationControl as WebcamSnapshotConfig;
            folderLocation = config.SaveLocation;
            fileNamePrefix = config.FilenamePrefix;
            captureWidth = config.CaptureWidth;
            captureHeight = config.CaptureHeight;
            selectedCameraPath = config.SelectedCameraPath;
            selectedCameraName = config.SelectedCameraName;
            showPreview = config.ShowPreview;
            playShutterSound = config.PlayShutterSound;
        }

        public string GetConfigString()
        {
            return string.Format("\"{0}\" ", selectedCameraName);
        }
    }
}
