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

        // The device we are recording from
        private CameraDriver i = CameraDriver.Instance;
        private Camera cam = null;

        private string last_video_saved = String.Empty;

        private bool video_saving = true; 

        public VideoReaction()
        {
            Setup();
        }

        [OnDeserialized]
        public void Setup()
        {
        
        }



        public void SaveVideo()
        {
            Logger.WriteLine("SaveImage");
            DateTime now = DateTime.Now;
            // TODO think of a better naming convention
            string fileName = "Mayhem-Video_" + this.Name + "_" + now.Year + "_" + now.Month + "_" + now.Month + "_" + now.Day + "-" + now.Hour + "_" + now.Minute + "_" + now.Second + ".avi";
            string path = this.folderLocation + "\\" + fileName;
            last_video_saved = path; 
            Logger.WriteLine("saving file to " + path);
            //image.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
            if (Directory.Exists(folderLocation))
            {
                video_saving = true; 
                Video v = new Video(cam, path);
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

        public override void Perform()
        {
            if (!video_saving && cam != null)
                SaveVideo();
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

            capture_offset_time = config.slider_value;
        }
    }
}
