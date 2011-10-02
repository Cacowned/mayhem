/*
 * MotionDetectorEvent.cs
 * 
 * Motion Detector Mayhem Module
 * 
 * (c) 2011, Microsoft Applied Sciences Group
 * 
 * Author: Sven Kratz
 * 
 */
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading;
using System.Windows;
using MayhemCore;
using MayhemOpenCVWrapper;
using MayhemOpenCVWrapper.LowLevel;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using VisionModules.Wpf;

namespace VisionModules.Events
{
    [DataContract]
    [MayhemModule("Motion Detector", "Detects when there is motion in the frame")]
    public class MotionDetector : EventBase, IWpfConfigurable
    {
        [DataMember]
        private Rect boundingRect;

        // which cam have we selected
        [DataMember]
        private int selectedDeviceIndex;

        private DateTime lastMotionDetected = DateTime.Now;
        private const int detectionInterval = 5000; //ms

        private MotionDetectorComponent motionDetectorComponent;
        private bool firstFrame = true;

        private MotionDetectorComponent.DetectionHandler motionUpdateHandler;

        private CameraDriver cameraDriver;
        private Camera camera = null;

        protected override void OnLoadDefaults()
        {
            boundingRect = new Rect(0, 0, 0, 0);
        }

        protected override void OnAfterLoad()
        {
            Logger.WriteLine("Enumerating Devices");

            cameraDriver = CameraDriver.Instance;

            if (selectedDeviceIndex < cameraDriver.DeviceCount)
            {
                camera = cameraDriver.cameras_available[selectedDeviceIndex];
            }
            else
            {
                Logger.WriteLine("No camera available");
            }

            motionDetectorComponent = new MotionDetectorComponent(320, 240);
            motionUpdateHandler = new MotionDetectorComponent.DetectionHandler(m_OnMotionUpdate);

            if (boundingRect.Width > 0 && boundingRect.Height > 0)
            {
                motionDetectorComponent.SetMotionBoundaryRect(boundingRect);
            }
        }

        private void m_OnMotionUpdate(object sender, List<System.Drawing.Point> points)
        {
            TimeSpan ts = DateTime.Now - lastMotionDetected;

            if (ts.TotalMilliseconds > detectionInterval)
            {

                Logger.WriteLine("m_OnMotionUpdate");

                // trigger the reaction
                if (!firstFrame)
                    base.Trigger();
                else
                    firstFrame = false;

                lastMotionDetected = DateTime.Now;
            }
        }

        public string GetConfigString()
        {
            string conf = "";
            if (camera != null)
            {
                conf += "Camera: " + camera.Info.deviceId;
            }
            return conf;
        }

        public WpfConfiguration ConfigurationControl
        {
            get
            {
                MotionDetectorConfig config = new MotionDetectorConfig(this.camera); // pass the parameters to initially populate the window in the constructor
                config.DeviceList.SelectedIndex = selectedDeviceIndex;
                if (boundingRect.Width > 0 && boundingRect.Height > 0)
                {
                    config.selectedBoundingRect = boundingRect;
                }
                return config;
            }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            // Grab data from the window variable and store it in this class

            // set the selected bounding rectangle
            boundingRect = ((MotionDetectorConfig)configurationControl).selectedBoundingRect;
            motionDetectorComponent.SetMotionBoundaryRect(boundingRect);


            // assign selected cam
            camera = ((MotionDetectorConfig)configurationControl).selected_camera;

            selectedDeviceIndex = camera.Info.deviceId;

        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
            Logger.WriteLine("Enable");

            // TODO: Improve this code
            if (!e.WasConfiguring && selectedDeviceIndex < cameraDriver.DeviceCount)
            {
                camera = cameraDriver.cameras_available[selectedDeviceIndex];
                if (!camera.running)
                    camera.StartFrameGrabbing();
                firstFrame = true;
                // register the trigger's motion update handler
                motionDetectorComponent.RegisterForImages(camera);
                motionDetectorComponent.OnMotionUpdate -= motionUpdateHandler;
                motionDetectorComponent.OnMotionUpdate += motionUpdateHandler;
            }
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            Logger.WriteLine("Disable");
            if (camera != null && !e.IsConfiguring)
            {
                firstFrame = true;
                // de-register the trigger's motion update handler
                motionDetectorComponent.UnregisterForImages(camera);
                motionDetectorComponent.OnMotionUpdate -= motionUpdateHandler;
                // try to shut down the camera
                camera.TryStopFrameGrabbing();
            }
        }

    }
}
