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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using MayhemCore.ModuleTypes;
using System.Runtime.Serialization;
using System.Windows;
using VisionModules.Wpf;
using MayhemOpenCVWrapper;
using System.Drawing;
using System.Diagnostics;
using MayhemWpf.UserControls;
using System.Timers;

namespace VisionModules.Reactions
{
    [DataContract]
    [MayhemModule("Picture", "Takes a photo with a webcam and saves it to the hard drive")]
    public class Picture : ReactionBase, IWpfConfigurable
    {
        // default to "My Documents" folder
        [DataMember]
        public string folderLocation = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

        // The device we are recording from
        private CameraDriver i = CameraDriver.Instance;
        private ImagerBase cam;

        public Camera.ImageUpdateHandler imageUpdateHandler;

        [DataMember]
        private int selected_device_idx = 0;

        // the temporal offset of the picture to be saved
        [DataMember]
        private double capture_offset_time = 0.0;

        public Picture()
        {
            Setup();
        }

        [OnDeserialized]
        public void Deserialize(StreamingContext s)
        {
            Setup();
        }

        [OnSerializing]
        public void Serializing(StreamingContext s)
        {
            //Disable();
        }

        public void camera_update(object sender, EventArgs e)
        {

        }

        public void Setup()
        {
            imageUpdateHandler = new Camera.ImageUpdateHandler(camera_update);

            if (i == null)
            {
                i = CameraDriver.Instance;
            }

            if (selected_device_idx < i.devices_available.Length)
            {
                cam = i.cameras_available[selected_device_idx];
            }
        }

        /// <summary>
        /// gets called when a new image is acquired by the camera
        /// </summary>

        /// TODO: handle this callback in Camera.cs and encapsulate the pixel copying
        public void SaveImage(Bitmap image)
        {
            Logger.WriteLine("SaveImage");
            DateTime now = DateTime.Now;
            // TODO think of a better naming convention
            string filename = "Mayhem-Snapshot_" + this.Name + "_" + now.Year + "_" + now.Month + "_" + now.Month + "_" + now.Day + "-" + now.Hour + "_" + now.Minute + "_" + now.Second + ".jpg";
            string path = this.folderLocation + "\\" + filename;
            Logger.WriteLine("saving file to " + path);
            image.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
        }


        public override void Enable()
        {
            // TODO: Improve this code
            if (selected_device_idx < i.devices_available.Length)
            {
                cam = i.cameras_available[selected_device_idx];
                cam.OnImageUpdated += imageUpdateHandler;
                if (cam.running == false)
                    cam.StartFrameGrabbing();

            }

            base.Enable();
        }

        public override void Disable()
        {
            base.Disable();
            cam.OnImageUpdated -= imageUpdateHandler;
            cam.TryStopFrameGrabbing();
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
                int time_ms = (int)capture_offset_time * 1000;
                Timer t = new Timer(time_ms);
                t.Elapsed += new ElapsedEventHandler(SaveFutureImage);
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
            if (this.Enabled && cam.running)
                SaveImage(cam.ImageAsBitmap());
        }

        protected string DateTimeToTimeStamp(DateTime time)
        {
            return time.ToString("yyyyMMddHHmmssffff");
        }

        public IWpfConfiguration ConfigurationControl
        {
            get
            {
                PictureConfig config = new PictureConfig(folderLocation, capture_offset_time);
                config.deviceList.SelectedIndex = selected_device_idx;
                return config;
            }
        }

        public void OnSaved(IWpfConfiguration configurationControl)
        {
            PictureConfig config = configurationControl as PictureConfig;
            folderLocation = config.location;

            bool wasEnabled = this.Enabled;

            //if (this.Enabled)
            //    this.Disable();
            if (config.deviceList.HasItems)
            {
                // assign selected cam
                cam = config.deviceList.SelectedItem as Camera;
                selected_device_idx = config.deviceList.SelectedIndex;

                //   if (wasEnabled)
                //       this.Enable();
            }
            else
            {
                Logger.WriteLine("no cam present, using dummy");
                cam = new DummyCamera();
            }
            capture_offset_time = config.slider_value;
        }

        private new void SetConfigString()
        {
            ConfigString = String.Format("Save Location: \"{0}\"", folderLocation);
        }

    }
}
