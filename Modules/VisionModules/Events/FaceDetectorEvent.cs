/*
 * FaceDetectorEvent.cs
 * 
 * A basic Face Detector Event:
 * face detector now only triggers when the amount of faces surpasses a threshold value, and only once then. 
 * When the amount of faces goes below the threshold and then above it again, the reaction triggers again, and so forth. 
 * 
 * (c) 2011, Microsoft Applied Sciences Group
 * 
 * Author: Sven Kratz
 * 
 */ 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Windows;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using VisionModules.Wpf;
using MayhemOpenCVWrapper;
using System.Diagnostics;
using MayhemWpf.UserControls;
using MayhemOpenCVWrapper.LowLevel;
using System.Threading;

namespace VisionModules.Events
{
    [DataContract]
    [MayhemModule("Face Detector", "Detects if and how many faces are in the scene")]
    public class FaceDetectorEvent : EventBase, IWpfConfigurable
    {
        private const int detectionInterval = 2500; //ms
        private DateTime lastFacesDetectedTime = DateTime.Now;
        private FaceDetectorComponent fd;
        private FaceDetectorComponent.DetectionHandler faceDetectUpdateHandler;
        private CameraDriver i = CameraDriver.Instance;
        private Camera cam = null;
        private int lastFacesDetectedAmount = 0; 

        // the cam we have selected
        [DataMember]
        private int selected_device_idx = 0;

        [DataMember]
        private Rect boundingRect;

        [DataMember]
        private int triggerOnNrOfFaces = 1;

        protected override void Initialize()
        {
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

            fd = new FaceDetectorComponent();
            faceDetectUpdateHandler = new FaceDetectorComponent.DetectionHandler(m_onFaceDetectUpdate);
        }

        void m_onFaceDetectUpdate(object sender, List<System.Drawing.Point> points)
        {
            // number of faces is points.Size() / 4
            // as each faces is returned with its own bounding box
            int nrFacesDetected = points.Count / 2;
            Logger.WriteLine("m_onFaceDetected: count " + nrFacesDetected);   
            TimeSpan ts = DateTime.Now - lastFacesDetectedTime;
            if (points.Count > 0 && ts.TotalMilliseconds > detectionInterval)
            {
                // only trigger if the lastFacesDetectedAmount was smaller than the trigger threshold
                // and only if the number of faces detected this time is greater or equal then the trigger threshold
                if (lastFacesDetectedAmount < triggerOnNrOfFaces && nrFacesDetected >= triggerOnNrOfFaces)
                {                
                    base.Trigger();
                }
                lastFacesDetectedTime = DateTime.Now;             
            }
            lastFacesDetectedAmount = nrFacesDetected;
        }

        public string GetConfigString()
        {
            string config = "";
            if (cam != null)
            {
                config += "Camera: " + cam.Info.deviceId + ", ";
            }
           
            config += "Detect " + triggerOnNrOfFaces;
            if (triggerOnNrOfFaces == 1)
                config += " face";
            else
                config += " faces";
            return config; 
        }

        public WpfConfiguration ConfigurationControl
        {
            get
            {
                Logger.WriteLine("get ConfigurationControl!");
                FaceDetectConfig config = new FaceDetectConfig(this.cam);
                if (boundingRect.Width > 0 && boundingRect.Height > 0)
                {            
                    config.selectedBoundingRect = boundingRect;
                }

                config.DeviceList.SelectedIndex = selected_device_idx;
                return config;
            }
        }

        protected override bool OnEnable()
        {
            if (!IsConfiguring && selected_device_idx < i.DeviceCount)
            {
                cam = i.cameras_available[selected_device_idx];
                //if (cam.running == false)
                //Thread.Sleep(350);
                cam.StartFrameGrabbing();
                // register the trigger's faceDetection update handler
                fd.RegisterForImages(cam);
                fd.OnFaceDetected -= m_onFaceDetectUpdate;
                fd.OnFaceDetected += m_onFaceDetectUpdate;
            }
            return true;
        }

        protected override void OnDisable()
        {
            Logger.WriteLine("");       
            if (!IsConfiguring && cam != null)
            {
                fd.OnFaceDetected -= m_onFaceDetectUpdate;
                fd.UnregisterForImages(cam); 
                // de-register the trigger's faceDetection update handler                
                // try to shut down the camera           
                cam.TryStopFrameGrabbing();              
            }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {            
            Logger.WriteLine("OnSaved!");    
            //   folderLocation = ((FaceDetectConfig)configurationControl).location;
            bool wasEnabled = this.IsEnabled;
            if (this.IsEnabled)
                this.OnDisable();
            // assign selected cam
            cam = ((FaceDetectConfig)configurationControl).DeviceList.SelectedItem as Camera;

            // set the selected bounding rectangle
            boundingRect = ((MotionDetectorConfig)configurationControl).overlay.GetBoundingRect();
            Logger.WriteLine("BOUNDING RECT : " + boundingRect);
            triggerOnNrOfFaces = ((FaceDetectConfig)configurationControl).NumberOfFacesSelected;                      
            if (wasEnabled)
                this.OnEnable();
        }
    }
}
