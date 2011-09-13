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
using MayhemCore.ModuleTypes;
using VisionModules.Wpf;
using MayhemOpenCVWrapper;
using System.Collections.Generic;
using System.Diagnostics;
using MayhemDefaultStyles.UserControls;
using MayhemOpenCVWrapper.LowLevel;

namespace VisionModules.Events
{
    [DataContract]
    [MayhemModule("Motion Detector", "Detects when there is motion in the frame")]
    public class MotionDetector : EventBase, IWpfConfigurable
    {

        private const string TAG = "[MotionDetector] : ";
        private DateTime lastMotionDetected = DateTime.Now;
        private const int detectionInterval = 5000; //ms

        private MotionDetectorComponent m;
        
        [DataMember]
        Rect boundingRect = new Rect(0,0,0,0);


        private MotionDetectorComponent.DetectionHandler motionUpdateHandler;

        private CameraDriver i = CameraDriver.Instance;
        private Camera cam = null;

        // which cam have we selected
        [DataMember]
        private int selected_device_idx = 0;

        public MotionDetector()
        {       
            Initialize();
        }

       

        /** <summary>
         * Called when deserialized / on instantiation
         * </summary> */
        protected override void Initialize()
        {
            Debug.WriteLine(TAG + "Initialize");
            base.Initialize();

            Debug.WriteLine(TAG + "Enumerating Devices");

            if (i == null)
                i = CameraDriver.Instance; 
            
            if (selected_device_idx < i.devices_available.Length)
            {
                cam = i.cameras_available[selected_device_idx];
            }
            else
            {
                Debug.WriteLine(TAG + "No camera available");
            }

            m = new MotionDetectorComponent(320, 240);
            motionUpdateHandler = new MotionDetectorComponent.DetectionHandler(m_OnMotionUpdate);

            if (boundingRect.Width > 0 && boundingRect.Height > 0)
            {
                m.SetMotionBoundaryRect(boundingRect);
            }

            SetConfigString();



        }

        void m_OnMotionUpdate(object sender, List<System.Drawing.Point> points)
        {
            TimeSpan ts = DateTime.Now - lastMotionDetected;

            if (ts.TotalMilliseconds > detectionInterval)
            {

                Debug.WriteLine(TAG + "m_OnMotionUpdate");

                // trigger the reaction
                base.OnEventActivated();

                lastMotionDetected = DateTime.Now;
            }
        }


        protected new void SetConfigString()
        {
            ConfigString = String.Format("Configuration Message");
        }

        public IWpfConfiguration ConfigurationControl
        {
            get
            {
                MotionDetectorConfig config = new MotionDetectorConfig(); // pass the parameters to initially populate the window in the constructor
                config.DeviceList.SelectedIndex = selected_device_idx;
                if (boundingRect.Width > 0 && boundingRect.Height > 0)
                {
                    config.selectedBoundingRect = boundingRect; 
                }
                return config;
            }
        }

        public void OnSaved(IWpfConfiguration configurationControl)
        {
            // Grab data from the window variable and store
            // it in this class
            bool wasEnabled = this.Enabled;

            // set the selected bounding rectangle
            boundingRect = ((MotionDetectorConfig)configurationControl).selectedBoundingRect;
            m.SetMotionBoundaryRect(boundingRect); 

            if (this.Enabled)
                this.Disable();
            // assign selected cam
            cam = ((MotionDetectorConfig)configurationControl).selected_camera;

            if (wasEnabled)
                this.Enable();

            SetConfigString();
        }

        public override void Enable()
        {
            base.Enable();
            Debug.WriteLine(TAG + "Enable");

            // TODO: Improve this code
            if (selected_device_idx < i.devices_available.Length)
            {
                cam = i.cameras_available[selected_device_idx];
                if (cam.running == false)
                    cam.StartFrameGrabbing();

            }
            // register the trigger's motion update handler
            m.RegisterForImages(cam);
            m.OnMotionUpdate += motionUpdateHandler;

        }

        public override void Disable()
        {
            base.Disable();
            Debug.WriteLine(TAG + "Disable");
            // de-register the trigger's motion update handler
            m.UnregisterForImages(cam);
            m.OnMotionUpdate -= motionUpdateHandler;
            // try to shut down the camera
            cam.TryStopFrameGrabbing();
        }
       
    }
}
