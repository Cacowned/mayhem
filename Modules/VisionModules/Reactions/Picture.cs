/*
 * CamSnapshot.cs
 * 
 * Basic snapshot module that saves camera images to disk. 
 * 
 * (c) 2010/2011, Microsoft Applied Sciences Group
 * 
 * Author: Sven Kratz
 * 
 */

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Serialization;
using System.Timers;
using MayhemCore;
using MayhemOpenCVWrapper;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using VisionModules.Wpf;

namespace VisionModules.Reactions
{
    [DataContract]
    [MayhemModule("Picture", "Takes a photo with a webcam and saves it to the hard drive")]
    public class Picture : ReactionBase, IWpfConfigurable
    {
        // default to "My Documents" folder
        [DataMember]
        private string folderLocation = AppDomain.CurrentDomain.BaseDirectory;

        [DataMember]
        private int selected_device_idx = 0;

        [DataMember]
        private string fileNamePrefix = "Mayhem"; 

        // The device we are recording from
        private CameraDriver i = CameraDriver.Instance;
        private ImagerBase cam;

       

        // the temporal offset of the picture to be saved
        [DataMember]
        private double capture_offset_time = 0.0;

        protected override void  Initialize()
        {
        
            if (i == null)
            {
                i = CameraDriver.Instance;
            }

            if (selected_device_idx < i.DeviceCount)
            {
                cam = i.cameras_available[selected_device_idx];
            }
        }

        /// <summary>
        /// gets called when a new image is acquired by the camera
        /// </summary>
        public void SaveImage(Bitmap image)
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
            string path = this.folderLocation + "\\" + filename;
            Logger.WriteLine("saving file to " + path);
            image.Save(path, ImageFormat.Jpeg);
            
            // VERY important! 
            image.Dispose();
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
            // TODO: Improve this code
            if (!e.IsConfiguring && selected_device_idx < i.DeviceCount)
            {
                cam = i.cameras_available[selected_device_idx];
                //Thread.Sleep(350);
                cam.StartFrameGrabbing();
            }
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            if (!e.IsConfiguring && cam != null)
            {
                cam.TryStopFrameGrabbing();
            }
            //Thread.Sleep(350); 
        }

        public override void Perform()
        {
            Logger.WriteLine("Perform");
            // hook up image callback
            // image gets saved when image provider calls back
            // cam.OnImageUpdated += this.imageUpdateHandler;

            if (capture_offset_time == 0)
            {
                // save image directly
                SaveImage(cam.ImageAsBitmap());
            }
            else if (capture_offset_time < 0 && Math.Abs(capture_offset_time) <= Camera.LOOP_DURATION)
            {
                // retrieve image from camera buffer

                // buffer index = capture offset time / camera fram rate
                int buff_idx = (int)(-capture_offset_time * 1000 / (double)CameraSettings.DEFAULTS().updateRate_ms);

                if (cam is IBufferingImager)
                {
                    Bitmap image = ((IBufferingImager)cam).GetBufferItemAtIndex(buff_idx);
                    if (image != null)
                    {
                        SaveImage(image);
                    }
                }
            }
            else if ((capture_offset_time > 0 && Math.Abs(capture_offset_time) <= Camera.LOOP_DURATION))
            {
                // schedule future retrieval of image
                double time_ms = capture_offset_time * 1000;
                System.Timers.Timer t = new System.Timers.Timer(time_ms);
                t.Elapsed += new ElapsedEventHandler(SaveFutureImage);
                t.AutoReset = false; 
                t.Enabled = true;
             
            }
            else
            {
                // this branch should never be reached
                throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Event handler to be called when saving images in the future
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SaveFutureImage(object sender, ElapsedEventArgs e)
        {
            Logger.WriteLine("SaveFutureImage");
            if (this.IsEnabled && cam.running)
                SaveImage(cam.ImageAsBitmap());
        }

        protected string DateTimeToTimeStamp(DateTime time)
        {
            return time.ToString("yyyyMMddHHmmssffff");
        }

        public WpfConfiguration ConfigurationControl
        {
            get
            {
                PictureConfig config = new PictureConfig(folderLocation, fileNamePrefix, capture_offset_time, selected_device_idx);
                return config;
            }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            PictureConfig config = configurationControl as PictureConfig;
            folderLocation = config.SaveLocation;
            fileNamePrefix = config.FilenamePrefix;

            bool wasEnabled = this.IsEnabled;

            int camera_index = config.SelectedDeviceIdx; 

            if (i.cameras_available.Count > camera_index)
            {
                // assign selected cam
                cam = i.cameras_available[camera_index];
                selected_device_idx = camera_index;
            }
            else
            {
                Logger.WriteLine("no cam present, using dummy");
                cam = new DummyCamera();
            }
            capture_offset_time = config.slider_value;
        }

        public string GetConfigString()
        {
            return String.Format("Save Location: \"{0}\"", folderLocation);
        }
    }
}
