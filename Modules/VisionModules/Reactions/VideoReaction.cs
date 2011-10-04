/*
 * Video.cs
 * 
 * This vision module records an avi video of the camera before or after an event has fired.
 * 
 * (c) 2011, Microsoft Applied Sciences Group
 *
 * Parts of the code make use of functions from the AviFile library by Corrina John
 * http://www.codeproject.com/KB/audio-video/avifilewrapper.aspx
 * 
 * Author: Sven Kratz
 * 
 */
using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using MayhemCore;
using MayhemOpenCVWrapper;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using VisionModules.Wpf;
using MayhemOpenCVWrapper.LowLevel;
using System.Timers;

namespace VisionModules.Reactions
{

    public enum VIDEO_RECORDING_MODE
    {
        PRE_EVENT = 0,                                        // record 30s prior to the event
        POST_EVENT = Camera.LoopDuration / 1000,             // record 30s after event
        MID_EVENT = (Camera.LoopDuration / 1000) / 2         // record 15s before and 15s after the event
    }

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
        private VIDEO_RECORDING_MODE videoRecordingMode;

        [DataMember]
        private bool shouldCompress;

        // The device we are recording from
        private CameraDriver cameraDriver = CameraDriver.Instance;
        private Camera camera = null;
        private DummyCameraImageListener dummyCameraListener = new DummyCameraImageListener();

        private string lastVideoSaved = String.Empty;

        private bool videoSaving = false;

        protected override void OnLoadDefaults()
        {
            cameraDriver = CameraDriver.Instance;
            dummyCameraListener = new DummyCameraImageListener();
            folderLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            fileNamePrefix = "Mayhem";
            selectedDeviceIndex = 0;
            videoRecordingMode = VIDEO_RECORDING_MODE.MID_EVENT;
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
            Logger.WriteLine("SaveVideo");
            DateTime now = DateTime.Now;
            string fileName = fileNamePrefix + "_" +
                now.Year.ToString("D2") + "_" +
                now.Month.ToString("D2") + "_" +
                now.Day.ToString("D2") + "-" +
                now.Hour.ToString("D2") + "_" +
                now.Minute.ToString("D2") + "_" +
                now.Second.ToString("D2") + ".avi";
            string path = this.folderLocation + "\\" + fileName;
            lastVideoSaved = path;
            Logger.WriteLine("saving file to " + path);
            if (Directory.Exists(folderLocation))
            {
                videoSaving = true;
                Video v = new Video(camera, path, shouldCompress);
                v.OnVideoSaved += new Action<bool>(v_OnVideoSaved);
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
        /// <param name="obj"></param>
        private void v_OnVideoSaved(bool obj)
        {
            videoSaving = false;
            Logger.WriteLine("Video saved successfully to: " + lastVideoSaved);
        }

        /// <summary>
        /// When triggered, start write of video to disk
        /// video_saving flags video saving mode, so that repeat triggers won't save
        /// multiple copies of the same video
        /// </summary>
        public override void Perform()
        {
            int capture_offset_time = (int)videoRecordingMode;
            Logger.WriteLine("Capturing with offset " + capture_offset_time);
            if (!videoSaving && camera != null)
            {
                videoSaving = true;
                if (capture_offset_time == 0)
                {
                    SaveVideo(this, null);
                }
                else
                {
                    Logger.WriteLine("Recording Video with offset: " + capture_offset_time + "s");
                   // Timer t = new Timer(new TimerCallback((object state) => { SaveVideo(); }), this, capture_offset_time * 1000, Timeout.Infinite);
                    System.Timers.Timer t = new System.Timers.Timer(capture_offset_time * 1000);
                    t.Elapsed +=new System.Timers.ElapsedEventHandler(this.SaveVideo);
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
            Logger.WriteLine("");
            if (selectedDeviceIndex < cameraDriver.DeviceCount)
            {
                camera = cameraDriver.CamerasAvailable[selectedDeviceIndex];
                dummyCameraListener.RegisterForImages(camera);
                if (camera.Running == false)
                    camera.StartFrameGrabbing();
            }
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            if (camera != null)
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
            bool wasEnabled = this.IsEnabled;

            VideoConfig config = configurationControl as VideoConfig;
            folderLocation = config.SaveLocation;
            shouldCompress = config.compress_video;
            fileNamePrefix = config.FilenamePrefix;

            int camera_index = config.SelectedDeviceIdx;

            if (cameraDriver.CamerasAvailable.Count > camera_index)
            {
                camera = cameraDriver.CamerasAvailable[camera_index];
                selectedDeviceIndex = camera_index;
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
