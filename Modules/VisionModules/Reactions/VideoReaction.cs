﻿/*
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

namespace VisionModules.Reactions
{

    public enum VIDEO_RECORDING_MODE
    {
        PRE_EVENT = 0,                                        // record 30s prior to the event
        POST_EVENT = Camera.LOOP_DURATION / 1000,             // record 30s after event
        MID_EVENT = (Camera.LOOP_DURATION / 1000) / 2         // record 15s before and 15s after the event
    }

    [DataContract]
    [MayhemModule("Video", "Records an avi video of the camera scene before or after an event has fired")]
    public class VideoReaction : ReactionBase, IWpfConfigurable
    {
        [DataMember]
        private string folderLocation = AppDomain.CurrentDomain.BaseDirectory;

        [DataMember]
        private string fileNamePrefix = "Mayhem";

        [DataMember]
        private int selected_device_idx = 0;

        [DataMember]
        private VIDEO_RECORDING_MODE videoRecordingMode = VIDEO_RECORDING_MODE.MID_EVENT;

        [DataMember]
        private bool compress = false;

        // The device we are recording from
        private CameraDriver i = CameraDriver.Instance;
        private Camera cam = null;

        private string last_video_saved = String.Empty;

        private bool video_saving = false;

        protected override void Initialize()
        {
            Logger.WriteLine("");
            if (i == null)
            {
                i = CameraDriver.Instance;
            }

            if (selected_device_idx < i.DeviceCount)
            {
                Logger.WriteLine("Startup with camera " + selected_device_idx);
                cam = i.cameras_available[selected_device_idx];
            }
        }

        public string GetConfigString()
        {
            return folderLocation;
        }

        public void SaveVideo()
        {
            Logger.WriteLine("SaveImage");
            DateTime now = DateTime.Now;
            // TODO think of a better naming convention
            string fileName = fileNamePrefix  + "_" + 
                now.Year.ToString("D2") + "_" +
                now.Month.ToString("D2") + "_" +
                now.Day.ToString("D2") + "-" +
                now.Hour.ToString("D2") + "_" +
                now.Minute.ToString("D2") + "_" +
                now.Second.ToString("D2") + ".avi";
            string path = this.folderLocation + "\\" + fileName;
            last_video_saved = path;
            Logger.WriteLine("saving file to " + path);
            //image.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
            if (Directory.Exists(folderLocation))
            {
                video_saving = true;
                Video v = new Video(cam, path, compress);
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
            video_saving = false;
            Logger.WriteLine("Video saved successfully to: " + last_video_saved);
        }

        /// <summary>
        /// When triggered, start write of video to disk
        /// video_saving flags video saving mode, so that repeat triggers won't save
        /// multiple copies of the same video
        /// </summary>
        public override void Perform()
        {
            int capture_offset_time = (int) videoRecordingMode;
            Logger.WriteLine("Capturing with offset " + capture_offset_time);
            if (!video_saving && cam != null)
            {
                video_saving = true;
                if (capture_offset_time == 0)
                {
                    SaveVideo();
                }
                else
                {
                    Logger.WriteLine("Recording Video with offset: " + capture_offset_time + "s");
                    Timer t = new Timer(new TimerCallback((object state) => { SaveVideo(); }), this, capture_offset_time * 1000, Timeout.Infinite);
                }
            }
            else
            {
                Logger.WriteLine("Currently saving video: ignoring perform()");
            }
        }

        protected override bool OnEnable()
        {
            Logger.WriteLine("");
            if (selected_device_idx < i.DeviceCount)
            {
                cam = i.cameras_available[selected_device_idx];
                //cam.OnImageUpdated += imageUpdateHandler;
                if (cam.running == false)
                    cam.StartFrameGrabbing();
            }

            return true;
        }

        protected override void OnDisable()
        {
            if (cam != null)
            {
                cam.TryStopFrameGrabbing();
            }
        }

        public WpfConfiguration ConfigurationControl
        {
            get
            {
                return new VideoConfig(folderLocation, fileNamePrefix, videoRecordingMode, selected_device_idx);
            }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            bool wasEnabled = this.IsEnabled;
            if (this.IsEnabled)
                this.OnDisable();

            VideoConfig config = configurationControl as VideoConfig;
            folderLocation = config.SaveLocation;
            compress = config.compress_video;       
            fileNamePrefix = config.FilenamePrefix;

            int camera_index = config.SelectedDeviceIdx;

            if (i.cameras_available.Count > camera_index)
            {
                cam = i.cameras_available[camera_index];
                selected_device_idx = camera_index;
            }
            else
            {
                // TODO: Dummy Camera? 
                Logger.WriteLine("No Camera present, setting cam to null");
                cam = null;
            }

            videoRecordingMode = config.RecordingMode;

            if (wasEnabled)
                this.OnEnable();
        }
    }
}
