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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemCore.ModuleTypes;
using MayhemOpenCVWrapper;
using System.IO;
using System.Windows;
using VisionModules.Wpf;
using MayhemWpf.UserControls;
using System.Threading;

namespace VisionModules.Reactions
{
    [DataContract]
    [MayhemModule("Video", "Records an avi video of the camera scene before or after an event has fired")]
    public class VideoReaction : ReactionBase, IWpfConfigurable
    {

        [DataMember]
        private string folderLocation = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

        [DataMember]
        private int selected_device_idx = 0;

        [DataMember]
        private double capture_offset_time = 0;

        [DataMember]
        private bool compress = false;

        // The device we are recording from
        private CameraDriver i = CameraDriver.Instance;
        private Camera cam = null;

        private string last_video_saved = String.Empty;

        private bool video_saving = false; 

        public VideoReaction()
        {
            Setup(new StreamingContext());
        }

        [OnDeserialized]
        public void Setup(StreamingContext s)
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

        public void SaveVideo()
        {
            Logger.WriteLine("SaveImage");
            DateTime now = DateTime.Now;
            // TODO think of a better naming convention
            string fileName = "Mayhem-Video_" + now.Year + "_" + now.Month + "_" + now.Day + "-" + now.Hour + "_" + now.Minute + "_" + now.Second + ".avi";
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
            Logger.WriteLine("");
            video_saving = false;
            MessageBox.Show("Video Saved to: " + last_video_saved);
        }

        /// <summary>
        /// When triggered, start write of video to disk
        /// video_saving flags video saving mode, so that repeat triggers won't save
        /// multiple copies of the same video
        /// </summary>
        public override void Perform()
        {
            Logger.WriteLine("");
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
                    Timer t = new Timer(new TimerCallback((object state) => { SaveVideo(); }), this, (int)(capture_offset_time * 1000), Timeout.Infinite);

                }
            }
            else
            {
                Logger.WriteLine("Currently saving video: ignoring perform()");
            }
        }

        public override bool Enable()
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

        public override void Disable()
        {
            base.Disable();
            if (cam != null)
            {
                cam.TryStopFrameGrabbing();
            }
        }

        public IWpfConfiguration ConfigurationControl
        {
            get {
                    return new VideoConfig(folderLocation, capture_offset_time);
                }
        }

        public void OnSaved(IWpfConfiguration configurationControl)
        {

            bool wasEnabled = this.Enabled;
            if (this.Enabled)
                this.Disable();

            VideoConfig config = configurationControl as VideoConfig;
            folderLocation = config.location;
            compress = config.compress_video; 

            if (config.deviceList.HasItems)
            {
                cam = config.deviceList.SelectedItem as Camera;
                selected_device_idx = config.deviceList.SelectedIndex;
            }
            else
            {
                Logger.WriteLine("No Camera present, setting cam to null");
                cam = null; 
            }

            capture_offset_time = config.temporal_offset;

            if (wasEnabled)
                this.Enable();
        }
    }
}
