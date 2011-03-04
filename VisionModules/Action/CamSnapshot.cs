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

namespace VisionModules.Action
{
    [Serializable]
    public class CamSnapshot : ReactionBase, IWpf, ISerializable
    {
        public static string TAG = "[VisionModules.Action.CamSnapshot] : ";

        protected string folderLocation = "";

        // The device we are recording from

        private MayhemImageUpdater i = MayhemImageUpdater.Instance;
        private MayhemImageUpdater.ImageUpdateHandler imageUpdateHandler;

        private int selected_device = 0; 

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
            imageUpdateHandler = new MayhemImageUpdater.ImageUpdateHandler(i_OnImageUpdated);


        }

        /// <summary>
        /// gets called when a new image is acquired by the camera
        /// </summary>
        public void i_OnImageUpdated(object sender, EventArgs e)
        {
            // Executes Action
            // avoid capturing any more images
            i.OnImageUpdated -= imageUpdateHandler;
            // access image buffer

            Bitmap BackBuffer = new Bitmap(320, 240, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            System.Drawing.Rectangle rect = new System.Drawing.Rectangle(0, 0, 320, 240);

            // get at the bitmap data in a nicer way
            System.Drawing.Imaging.BitmapData bmpData =
                BackBuffer.LockBits(rect, System.Drawing.Imaging.ImageLockMode.ReadWrite,
                BackBuffer.PixelFormat);

            int bufSize = i.bufSize;
            IntPtr ImgPtr = bmpData.Scan0;

            // grab the image

            lock (i.thread_locker)
            {
                // Copy the RGB values back to the bitmap
                System.Runtime.InteropServices.Marshal.Copy(i.imageBuffer, 0, ImgPtr, bufSize);
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

           // TODO: Start Cam
           // first check if that cam is still attached
            if (selected_device < i.devices_available.Length)
            {
                if (i.running == false)
                    i.StartFrameGrabbing();
            }

           
           
        }

        public override void Disable()
        {
            base.Disable();
           
            // unhook image callback
            i.OnImageUpdated -= this.imageUpdateHandler;
        }

        public override void Perform()
        {

            // hook up image callback 

            i.OnImageUpdated += this.imageUpdateHandler;

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

            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            if (window.ShowDialog() == true)
            {

                folderLocation = window.location;

                // TODO
                // webcam.Device = window.captureDevice;

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

            Setup();
        }

        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("FolderLocation", folderLocation);
        }
        #endregion
    }
}
