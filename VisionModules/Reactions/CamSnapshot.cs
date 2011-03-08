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

namespace VisionModules.Reactions
{
    [Serializable]
    public class CamSnapshot : ReactionBase, IWpf, ISerializable
    {
        public static string TAG = "[VisionModules.Reaction.CamSnapshot] : ";

        protected string folderLocation = "";

        // The device we are recording from

        private MayhemCameraDriver i = MayhemCameraDriver.Instance;
        private Camera.ImageUpdateHandler imageUpdateHandler;
        private Camera cam; 

        private int selected_device_idx = 0; 

        public CamSnapshot()
            : base("Webcam Snapshot (OpenCV)", "Takes a photo with your webcam and saves it to the hard drive.")
        {
            Setup();
        }

        public void Setup()
        {

            hasConfig = true;
            // TODO: What if we have multiple of these?
           
            SetConfigString();
            imageUpdateHandler = new Camera.ImageUpdateHandler(i_OnImageUpdated);

            
            if (selected_device_idx < i.devices_available.Length)
            {
                cam = i.cameras_available[selected_device_idx];
            } 


        }

        /// <summary>
        /// gets called when a new image is acquired by the camera
        /// </summary>
        public void i_OnImageUpdated(object sender, EventArgs e)
        {
            // Executes Action
            // avoid capturing any more images
            cam.OnImageUpdated -= imageUpdateHandler;
            // access image buffer

            Bitmap BackBuffer = new Bitmap(320, 240, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, 320, 240);

            // get at the bitmap data in a nicer way
            System.Drawing.Imaging.BitmapData bmpData =
                BackBuffer.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                BackBuffer.PixelFormat);

            int bufSize = cam.bufSize;
            IntPtr ImgPtr = bmpData.Scan0;

            // grab the image

            lock (cam.thread_locker)
            {
                // Copy the RGB values back to the bitmap
                System.Runtime.InteropServices.Marshal.Copy(cam.imageBuffer, 0, ImgPtr, bufSize);
            }
            // Unlock the bits.
            BackBuffer.UnlockBits(bmpData);
            DateTime now = DateTime.Now;
            // TODO think of a better naming convention
            string filename = "Mayhem-Snapshot_" + this.Name + "_" + now.Year + "_" + now.Month + "_" + now.Month + "_" + now.Day + "-" + now.Hour + "_" + now.Minute + "_" + now.Second + ".jpg";
            string path = this.folderLocation + "\\" + filename;
            Debug.WriteLine(TAG + "saving file to " + path);
            BackBuffer.Save(path, System.Drawing.Imaging.ImageFormat.Jpeg);
        }


        public override void Enable()
        {
            base.Enable();

           // TODO: Improve this code
            if (selected_device_idx < i.devices_available.Length)
            {
                cam = i.cameras_available[selected_device_idx];
                if (cam.running == false)
                   cam.StartFrameGrabbing();
            }
           
           
        }

        public override void Disable()
        {
            base.Disable();
           
            // unhook image callback
            cam.OnImageUpdated -= this.imageUpdateHandler;

            cam.TryStopFrameGrabbing();
        }

        public override void Perform()
        {

            // hook up image callback 


            cam.OnImageUpdated += this.imageUpdateHandler;

            // image gets saved when image provider calls back

        }

        protected string DateTimeToTimeStamp(DateTime time)
        {
            return time.ToString("yyyyMMddHHmmssffff");
        }

        public void WpfConfig()
        {
            /*
             * Choose a webcam and save folder
             */
            var window = new CamSnapshotConfig(folderLocation, null /* cameraDevice */);

            window.DeviceList.SelectedIndex = selected_device_idx;

            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            if (window.ShowDialog() == true)
            {

                folderLocation = window.location;

                bool wasEnabled = this.Enabled;

                if (this.Enabled) this.Disable();
                // assign selected cam
                cam = window.DeviceList.SelectedItem as Camera;

                if (wasEnabled) this.Enable();
                

                SetConfigString();
            }

        }

        private void SetConfigString()
        {
            ConfigString = String.Format("Save Location: \"{0}\"", folderLocation);
        }

        #region Serialization
        public CamSnapshot(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

            

            folderLocation = info.GetString("FolderLocation");


            string camera_description = "";

            // try to initialize the camera from the camera ID
            try
            {
                selected_device_idx = info.GetInt32("CameraID");
                camera_description = info.GetString("CameraName");
            }
            catch (Exception ex)
            {
                selected_device_idx = 0; 

            }

            // see if the particular cam is still present

            if (selected_device_idx < i.cameras_available.Length)
            {
                if (camera_description.Equals(i.cameras_available[selected_device_idx].info.description))
                {
                    // great!, do nothing
                }
                else
                {
                    // default cam
                    selected_device_idx = 0;
                }
            }

            

            Setup();
        }

        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            try
            {
                info.AddValue("FolderLocation", folderLocation);
                // may be problematic if user's setup changes between startups of mayhem
                info.AddValue("CameraID", (Int32)cam.info.deviceId);
                info.AddValue("CameraName", cam.info.description);
            }
            catch (Exception ex) { }
        }
        #endregion
    }
}
