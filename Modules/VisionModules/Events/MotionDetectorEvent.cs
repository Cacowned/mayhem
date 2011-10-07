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

namespace VisionModules.Events
{
    [DataContract]
    [MayhemModule("Motion Detector", "Detects when there is motion in the frame")]
    public class MotionDetector : EventBase, IWpfConfigurable
    {
        private DateTime lastMotionDetected = DateTime.Now;
        private const int detectionInterval = 5000; //ms

        private MotionDetectorComponent m;
        private bool firstFrame = true; 
        
        [DataMember]
        private Rect boundingRect;

        private CameraDriver i = CameraDriver.Instance;
        private Camera cam = null;

        // which cam have we selected
        [DataMember]
        private int selected_device_idx;

        [DataMember]
        private double sensitivity;

        protected override void Initialize()
        {
            Logger.WriteLine("Enumerating Devices");

            if (i == null)
                i = CameraDriver.Instance;

            if (selected_device_idx < i.DeviceCount)
            {
                cam = i.cameras_available[selected_device_idx];
            }
            else
            {
                Logger.WriteLine("No camera available");
            }

            m = new MotionDetectorComponent(320, 240);
            
            if (sensitivity != 0)
                m.Sensitivity = sensitivity;
            else
                m.Sensitivity = 5;

            if (boundingRect.Width > 0 && boundingRect.Height > 0)
            {
                m.SetMotionBoundaryRect(boundingRect);
            }
            else
            {
                m.SetMotionBoundaryRect(new Rect(0, 0, 320, 240));
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
            if (cam != null)
            {
                conf += "Camera: " + cam.Info.deviceId;
            }
            return conf; 
        }

        public WpfConfiguration ConfigurationControl
        {
            get
            {
                MotionDetectorConfig config = new MotionDetectorConfig(this.cam); // pass the parameters to initially populate the window in the constructor
                config.DeviceList.SelectedIndex = selected_device_idx;
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
            // Grab data from the window variable and store
            // it in this class
            bool wasEnabled = this.Enabled;

            // set the selected bounding rectangle
            boundingRect = ((MotionDetectorConfig)configurationControl).selectedBoundingRect;
            m.SetMotionBoundaryRect(boundingRect);

            if (this.Enabled)
            {
                this.Disable();
                Thread.Sleep(350);
            }

            // assign selected cam
            cam = ((MotionDetectorConfig)configurationControl).selected_camera;

            m.Sensitivity = ((MotionDetectorConfig)configurationControl).sensitivity;
            sensitivity = m.Sensitivity;

            selected_device_idx = cam.Info.deviceId;

            if (wasEnabled)
                this.Enable();
        }

        public override bool Enable()
        {
            Logger.WriteLine("Enable");

            // TODO: Improve this code
            if (!IsConfiguring && selected_device_idx < i.DeviceCount)
            {
                cam = i.cameras_available[selected_device_idx];
                if (!cam.running)
                    cam.StartFrameGrabbing();
                firstFrame = true; 
                // register the trigger's motion update handler
                m.RegisterForImages(cam);
                m.OnMotionUpdate -= OnMotionUpdated;
                m.OnMotionUpdate += OnMotionUpdated;             
            }

            return true;
        }

        public override void Disable()
        {
            base.Disable();
            Logger.WriteLine("Disable");   
            if (cam != null && !IsConfiguring)
            {
                firstFrame = true; 
                // de-register the trigger's motion update handler
                m.UnregisterForImages(cam);
                m.OnMotionUpdate -= OnMotionUpdated;
                // try to shut down the camera
                cam.TryStopFrameGrabbing();
            }
        }

    }
}
