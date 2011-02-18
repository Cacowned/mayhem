﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using MayhemCore.ModuleTypes;
using System.Runtime.Serialization;
using DefaultModules.WebcamHelpers;
using System.Drawing;
using DefaultModules.Wpf;
using System.Windows;
using System.IO;
using System.Drawing.Imaging;
using System.Diagnostics;

namespace DefaultModules.Reactions
{
    [Serializable]
    public class WebcamSnapshot : ReactionBase, IWpf, ISerializable
    {
        protected string folderLocation = "";

        // The device we are recording from
        protected Device cameraDevice;

        protected Webcam webcam;

        public WebcamSnapshot()
            : base("Webcam Snapshot", "Takes a photo with your webcam and saves it to the hard drive.") {

            
            Setup();
        }

        public void Setup() {

            hasConfig = true;
            // TODO: What if we have multiple of these?
            webcam = new Webcam();
            SetConfigString();
        }

        public override void Enable() {
            base.Enable();

            if (webcam.Device == null) {
                WpfConfig();
            }

            webcam.Start();
        }

        public override void Disable() {
            base.Disable();
            webcam.Stop();
        }

        public override void Perform() {
            Bitmap captureImage = webcam.Capture();

            if (captureImage == null) {
                Debug.WriteLine("[Webcam] No Images yet.");
                return;
            }

            string location = Path.Combine(folderLocation, DateTimeToTimeStamp(DateTime.Now)) + ".jpg";
            
            captureImage.Save(location, ImageFormat.Jpeg);
        }

        protected string DateTimeToTimeStamp(DateTime time) {
            return time.ToString("yyyyMMddHHmmssffff");
        }

        public void WpfConfig() {
            /*
             * Choose a webcam and save folder
             */
            var window = new WebcamSnapshotConfig(folderLocation, cameraDevice);
            
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            if (window.ShowDialog() == true) {

                folderLocation = window.location;

                webcam.Device = window.captureDevice;

                SetConfigString();
            }
            // Choose camera
            
        }

        private void SetConfigString() {
            ConfigString = String.Format("Save Location: \"{0}\"", folderLocation);
        }

        #region Serialization
        public WebcamSnapshot(SerializationInfo info, StreamingContext context)
            : base(info, context) {

            folderLocation = info.GetString("FolderLocation");

            Setup();
        }

        public new void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);
            info.AddValue("FolderLocation", folderLocation);
        }
        #endregion
    }
}
