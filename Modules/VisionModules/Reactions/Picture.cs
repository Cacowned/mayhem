﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.CompilerServices;
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
    /// <summary>
    /// Basic snapshot module that saves camera images to disk.
    /// </summary>
    [DataContract]
    [MayhemModule("Picture", "Takes a photo with a webcam and saves it to the hard drive")]
    public class Picture : ReactionBase, IWpfConfigurable
    {
        [DataMember]
        private string folderLocation;

        [DataMember]
        private int selectedDeviceIndex;

        [DataMember]
        private string fileNamePrefix;

        // the temporal offset of the picture to be saved
        [DataMember]
        private double captureOffsetTime;

        // The device we are recording from
        private CameraDriver cameraDriver;
        private ImagerBase camera;
        private DummyCameraImageListener dummyListener;

        protected override void OnLoadDefaults()
        {
            cameraDriver = CameraDriver.Instance;
            dummyListener = new DummyCameraImageListener();
            captureOffsetTime = 0.0;
            folderLocation = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            fileNamePrefix = "Mayhem";
            selectedDeviceIndex = 0;
        }

        protected override void OnAfterLoad()
        {
            dummyListener = new DummyCameraImageListener();
            cameraDriver = CameraDriver.Instance;
            if (selectedDeviceIndex < cameraDriver.DeviceCount)
            {
                camera = cameraDriver.CamerasAvailable[selectedDeviceIndex];
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
            string path = folderLocation + "\\" + filename;
            Logger.WriteLine("saving file to " + path);
            image.Save(path, ImageFormat.Jpeg);

            // VERY important! 
            image.Dispose();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected override void OnEnabling(EnablingEventArgs e)
        {
            cameraDriver = CameraDriver.Instance;

            if (!e.WasConfiguring && selectedDeviceIndex < cameraDriver.DeviceCount)
            {
                camera = cameraDriver.CamerasAvailable[selectedDeviceIndex];
                dummyListener.RegisterForImages(camera);
            }

            if (camera.Running == false)
                camera.StartFrameGrabbing();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected override void OnDisabled(DisabledEventArgs e)
        {
            if (!e.IsConfiguring && camera != null)
            {
                dummyListener.UnregisterForImages(camera); 
                camera.TryStopFrameGrabbing();
            }
        }

        public override void Perform()
        {
            if (captureOffsetTime == 0)
            {
                // save image directly
                SaveImage(camera.ImageAsBitmap());
            }
            else if (captureOffsetTime < 0 && Math.Abs(captureOffsetTime) <= Camera.LoopDuration)
            {
                // retrieve image from camera buffer
                // buffer index = capture offset time / camera fram rate
                int bufferIndex = (int)(-captureOffsetTime * 1000.0 / Camera.LoopBufferUpdateMs);

                if (camera is IBufferingImager)
                {
                    Bitmap image = ((IBufferingImager)camera).GetBufferItemAtIndex(bufferIndex);
                    if (image != null)
                    {
                        SaveImage(image);
                    }
                }
            }
            else if (captureOffsetTime > 0 && Math.Abs(captureOffsetTime) <= Camera.LoopDuration)
            {
                // schedule future retrieval of image
                double timeMs = captureOffsetTime * 1000;
                Timer t = new Timer(timeMs);
                t.Elapsed += SaveFutureImage;
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
            if (IsEnabled && camera.Running)
                SaveImage(camera.ImageAsBitmap());
        }

        protected string DateTimeToTimeStamp(DateTime time)
        {
            return time.ToString("yyyyMMddHHmmssffff");
        }

        public WpfConfiguration ConfigurationControl
        {
            get
            {
                PictureConfig config = new PictureConfig(folderLocation, fileNamePrefix, captureOffsetTime, selectedDeviceIndex);
                return config;
            }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            PictureConfig config = configurationControl as PictureConfig;
            folderLocation = config.SaveLocation;
            fileNamePrefix = config.FilenamePrefix;

            int cameraIndex = config.SelectedDeviceIdx;

            if (cameraDriver.CamerasAvailable.Count > cameraIndex)
            {
                // unregister, because camera might have changed
                dummyListener.UnregisterForImages(camera);
                camera = cameraDriver.CamerasAvailable[cameraIndex];
                dummyListener.RegisterForImages(camera);
                selectedDeviceIndex = cameraIndex;
            }
            else
            {
                Logger.WriteLine("no cam present, using dummy");
                camera = new DummyCamera();
            }

            captureOffsetTime = config.SliderValue;
        }

        public string GetConfigString()
        {
            return string.Format("Save Location: \"{0}\"", folderLocation);
        }
    }
}
