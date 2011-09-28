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
        private Rect boundingRect = new Rect(0,0,0,0);

        private MotionDetectorComponent.DetectionHandler motionUpdateHandler;

        private CameraDriver i = CameraDriver.Instance;
        private Camera cam = null;

        // which cam have we selected
        [DataMember]
        private int selected_device_idx;

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
            motionUpdateHandler = new MotionDetectorComponent.DetectionHandler(m_OnMotionUpdate);

            if (boundingRect.Width > 0 && boundingRect.Height > 0)
            {
                m.SetMotionBoundaryRect(boundingRect);
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
                return config;
            }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            // Grab data from the window variable and store
            // it in this class
            bool wasEnabled = this.IsEnabled;

            // set the selected bounding rectangle
            boundingRect = ((MotionDetectorConfig)configurationControl).selectedBoundingRect;
            m.SetMotionBoundaryRect(boundingRect);

            if (this.IsEnabled)
            {
                this.OnDisable();
                Thread.Sleep(350);
            }

            // assign selected cam
            cam = ((MotionDetectorConfig)configurationControl).selected_camera;

            selected_device_idx = cam.Info.deviceId;

            if (wasEnabled)
                this.OnEnable();
        }

        protected override bool OnEnable()
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
                m.OnMotionUpdate -= motionUpdateHandler;
                m.OnMotionUpdate += motionUpdateHandler;             
            }

            return true;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            Logger.WriteLine("Disable");   
            if (cam != null && !IsConfiguring)
            {
                firstFrame = true; 
                // de-register the trigger's motion update handler
                m.UnregisterForImages(cam); 
                m.OnMotionUpdate -= motionUpdateHandler;
                // try to shut down the camera
                cam.TryStopFrameGrabbing();
            }
        }

    }
}
