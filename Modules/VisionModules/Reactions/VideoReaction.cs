using System;
using System.IO;
using System.Runtime.Serialization;
using System.Timers;
using MayhemCore;
using MayhemOpenCVWrapper;
using MayhemOpenCVWrapper.LowLevel;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using VisionModules.Wpf;

namespace VisionModules.Reactions
{
    public enum VideoRecordingMode
    {
        PreEvent = 0,                                        // record 30s prior to the event
        PostEvent =  Camera.LoopDuration / 1000,             // record 30s after event
        MidEvent = (Camera.LoopDuration / 1000) / 2         // record 15s before and 15s after the event
    }

    /// <summary>
    /// This vision module records an avi video of the camera before or after an event has fired.
    /// 
    /// Parts of the code make use of functions from the AviFile library by Corrina John
    /// http://www.codeproject.com/KB/audio-video/avifilewrapper.aspx
    /// </summary>
    [DataContract]
    [MayhemModule("Video", "Records an avi video of the camera scene before or after an event has fired")]
    public class VideoReaction : ReactionBase, IWpfConfigurable
    {
        [DataMember]
        private string folderLocation;

        [DataMember]
        private string fileNamePrefix;

        [DataMember]
        private int selectedDeviceIndex;

        [DataMember]
        private VideoRecordingMode videoRecordingMode;

        [DataMember]
        private bool shouldCompress;

        // The device we are recording from
        private CameraDriver cameraDriver;
        private Camera camera;
        private DummyCameraImageListener dummyCameraListener;
        private string lastVideoSaved;
        private bool videoSaving;

        public VideoReaction()
        {
            lastVideoSaved = string.Empty;
            dummyCameraListener = new DummyCameraImageListener();
            cameraDriver = CameraDriver.Instance;
        }

        protected override void OnLoadDefaults()
        {
            cameraDriver = CameraDriver.Instance;
            dummyCameraListener = new DummyCameraImageListener();
            folderLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            fileNamePrefix = "Mayhem";
            selectedDeviceIndex = 0;
            videoRecordingMode = VideoRecordingMode.MidEvent;
            shouldCompress = false;
        }

        protected override void OnAfterLoad()
        {
            cameraDriver = CameraDriver.Instance;
            dummyCameraListener = new DummyCameraImageListener();
            if (selectedDeviceIndex < cameraDriver.DeviceCount)
            {
                Logger.WriteLine("Startup with camera " + selectedDeviceIndex);
                camera = cameraDriver.CamerasAvailable[selectedDeviceIndex];
            }
        }

        public string GetConfigString()
        {
            return folderLocation;
        }

        public void SaveVideo(object sender, ElapsedEventArgs e)
        {
            camera.IsRecordingVideo = false;
            Logger.WriteLine("SaveVideo");
            DateTime now = DateTime.Now;
            string fileName = fileNamePrefix + "_" +
                now.Year.ToString("D2") + "_" +
                now.Month.ToString("D2") + "_" +
                now.Day.ToString("D2") + "-" +
                now.Hour.ToString("D2") + "_" +
                now.Minute.ToString("D2") + "_" +
                now.Second.ToString("D2") + ".avi";
            string path = folderLocation + "\\" + fileName;
            lastVideoSaved = path;
            Logger.WriteLine("saving file to " + path);
            if (Directory.Exists(folderLocation))
            {
                videoSaving = true;
                Video v = new Video(camera, path, shouldCompress);
                v.OnVideoSaved += OnVideoSaved;
            }
            else
            {
                Logger.WriteLine("ERROR: Directory does not exist!");
                throw new FileNotFoundException();
            }
        }

        /// <summary>
        /// Called when the video is done saving itself
        /// </summary>
        private void OnVideoSaved(object sender, VideoSavedEventArgs e)
        {
            camera.IsRecordingVideo = true;
            videoSaving = false;
            if (e.SavedSuccessfully)
                Logger.WriteLine("Video saved successfully to: " + lastVideoSaved);
        }

        /// <summary>
        /// When triggered, start write of video to disk
        /// video_saving flags video saving mode, so that repeat triggers won't save
        /// multiple copies of the same video
        /// </summary>
        public override void Perform()
        {
            int captureOffsetTime = (int)videoRecordingMode;
            Logger.WriteLine("Capturing with offset " + captureOffsetTime);
            if (!videoSaving && camera != null)
            {
                videoSaving = true;
                if (captureOffsetTime == 0)
                {
                    SaveVideo(this, null);
                }
                else
                {
                    Logger.WriteLine("Recording Video with offset: " + captureOffsetTime + "s");

                    Timer t = new Timer(captureOffsetTime * 1000);
                    t.Elapsed += SaveVideo;
                    t.AutoReset = false;
                    t.Start();
                }
            }
            else
            {
                Logger.WriteLine("Currently saving video: ignoring perform()");
            }
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
            cameraDriver = CameraDriver.Instance;

            if (!e.WasConfiguring && selectedDeviceIndex < cameraDriver.DeviceCount)
            {
                camera = cameraDriver.CamerasAvailable[selectedDeviceIndex];
                dummyCameraListener.RegisterForImages(camera);
            }

            if (camera.Running == false)
                camera.StartFrameGrabbing();
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            if (!e.IsConfiguring && camera != null)
            {
                dummyCameraListener.UnregisterForImages(camera);
                camera.TryStopFrameGrabbing();
            }
        }

        public WpfConfiguration ConfigurationControl
        {
            get
            {
                return new VideoConfig(folderLocation, fileNamePrefix, videoRecordingMode, selectedDeviceIndex);
            }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            VideoConfig config = configurationControl as VideoConfig;
            folderLocation = config.SaveLocation;
            shouldCompress = config.CompressVideo;
            fileNamePrefix = config.FilenamePrefix;

            int cameraIndex = config.SelectedDeviceIdx;

            if (cameraDriver.CamerasAvailable.Count > cameraIndex)
            {
                // unregister, because camera might have changed
                dummyCameraListener.UnregisterForImages(camera);
                camera = cameraDriver.CamerasAvailable[cameraIndex];
                dummyCameraListener.RegisterForImages(camera);
                selectedDeviceIndex = cameraIndex;
            }
            else
            {
                Logger.WriteLine("No Camera present, setting cam to null");
                camera = null;
            }

            videoRecordingMode = config.RecordingMode;
        }
    }
}
