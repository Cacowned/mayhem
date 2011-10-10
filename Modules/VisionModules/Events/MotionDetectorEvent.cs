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
using System.Runtime.Serialization;
using System.Windows;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using VisionModules.Wpf;
using MayhemOpenCVWrapper;
using System.Collections.Generic;
using System.Diagnostics;
using MayhemWpf.UserControls;
using MayhemOpenCVWrapper.LowLevel;
using System.Threading;
using VisionModules.Events.Components;

namespace VisionModules.Events
{
    [DataContract]
    [MayhemModule("Motion Detector", "Detects when there is motion in the frame")]
    public class MotionDetector : EventBase, IWpfConfigurable
    {
        private DateTime lastMotionDetected = DateTime.Now;
        private const int detectionInterval = 5000; //ms

        private MotionDetectorComponent motionDetectorComponent;
        private bool firstFrame = true; 
        
        [DataMember]
        private Rect boundingRect;

        private CameraDriver cameraDriver = CameraDriver.Instance;
        private ImagerBase camera;

        // which cam have we selected
        [DataMember]
        private int selectedDeviceIndex;

        [DataMember]
        private double sensitivity;

        protected override void OnAfterLoad()
        {
            Logger.WriteLine("Enumerating Devices");

            cameraDriver = CameraDriver.Instance;

            if (selectedDeviceIndex < cameraDriver.DeviceCount)
            {
                camera = cameraDriver.CamerasAvailable[selectedDeviceIndex];
            }
            else
            {
                Logger.WriteLine("No camera available");
                camera = new DummyCamera();
            }

            motionDetectorComponent = new MotionDetectorComponent(camera);
            
            if (sensitivity != 0)
                motionDetectorComponent.Sensitivity = sensitivity;
            else
                motionDetectorComponent.Sensitivity = 5;

            if (boundingRect.Width > 0 && boundingRect.Height > 0)
            {
                motionDetectorComponent.SetMotionBoundaryRect(boundingRect);
            }
            else
            {
                motionDetectorComponent.SetMotionBoundaryRect(new Rect(0, 0, 320, 240));
            }
        }

        private void OnMotionUpdated(object sender, EventArgs e)
        {
            if (!firstFrame)
                base.Trigger();
                
            firstFrame = false;
        }

        public string GetConfigString()
        {
            string conf = ""; 
            if (camera != null)
            {
                conf += "Camera: " + camera.Info.DeviceId;
            }
            return conf; 
        }

        public WpfConfiguration ConfigurationControl
        {
            get
            {
                MotionDetectorConfig config = new MotionDetectorConfig(camera); // pass the parameters to initially populate the window in the constructor
                config.DeviceList.SelectedIndex = selectedDeviceIndex;
                if (boundingRect.Width > 0 && boundingRect.Height > 0)
                {
                    config.selectedBoundingRect = boundingRect; 
                }

                config.sensitivity = sensitivity;

                return config;
            }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            // set the selected bounding rectangle
            boundingRect = ((MotionDetectorConfig)configurationControl).selectedBoundingRect;
            motionDetectorComponent.SetMotionBoundaryRect(boundingRect);

            // assign selected cam
            camera = ((MotionDetectorConfig)configurationControl).selected_camera;

            motionDetectorComponent.Sensitivity = ((MotionDetectorConfig)configurationControl).sensitivity;
            sensitivity = motionDetectorComponent.Sensitivity;

            selectedDeviceIndex = camera.Info.DeviceId;
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
            base.OnEnabling(e);
        
            Logger.WriteLine("Enable");

            // TODO: Improve this code
            if (!e.WasConfiguring && selectedDeviceIndex < cameraDriver.DeviceCount)
            {
                camera = cameraDriver.CamerasAvailable[selectedDeviceIndex];
                if (!camera.Running)
                    camera.StartFrameGrabbing();
                firstFrame = true; 
                // register the trigger's motion update handler
                motionDetectorComponent.RegisterForImages(camera);
                motionDetectorComponent.OnMotionUpdate -= OnMotionUpdated;
                motionDetectorComponent.OnMotionUpdate += OnMotionUpdated;             
            }
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            base.OnDisabled(e);
            Logger.WriteLine("Disable");   
            if (camera != null && !e.IsConfiguring)
            {
                firstFrame = true; 
                // de-register the trigger's motion update handler
                motionDetectorComponent.UnregisterForImages(camera);
                motionDetectorComponent.OnMotionUpdate -= OnMotionUpdated;
                // try to shut down the camera
                camera.TryStopFrameGrabbing();
            }
        }

    }
}
