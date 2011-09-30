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
using System.Runtime.Serialization;
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
    [MayhemModule("Face Detector", "Detects if and how many faces are in the scene")]
    public class FaceDetectorEvent : EventBase, IWpfConfigurable
    {
        // the cam we have selected
        [DataMember]
        private int selectedDeviceIndex;

        [DataMember]
        private Rect boundingRect;

        [DataMember]
        private int triggerOnNrOfFaces;

        private const int detectionInterval = 2500; //ms
        private DateTime lastFacesDetectedTime = DateTime.Now;
        private FaceDetectorComponent faceDetector;
        private FaceDetectorComponent.DetectionHandler faceDetectUpdateHandler;
        private CameraDriver cameraDriver;
        private Camera cam = null;
        private int lastFacesDetectedAmount = 0;

        protected override void OnLoadDefaults()
        {
            selectedDeviceIndex = 0;
            triggerOnNrOfFaces = 1;
        }

        protected override void OnAfterLoad()
        {
            cameraDriver = CameraDriver.Instance;

            if (selectedDeviceIndex < cameraDriver.DeviceCount)
            {
                cam = cameraDriver.cameras_available[selectedDeviceIndex];
            }
            else
            {
                Logger.WriteLine("No camera available");
            }

            faceDetector = new FaceDetectorComponent();
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

                config.DeviceList.SelectedIndex = selectedDeviceIndex;
                return config;
            }
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
            if (!e.IsConfiguring && selectedDeviceIndex < cameraDriver.DeviceCount)
            {
                cam = cameraDriver.cameras_available[selectedDeviceIndex];
                //if (cam.running == false)
                //Thread.Sleep(350);
                cam.StartFrameGrabbing();
                // register the trigger's faceDetection update handler
                faceDetector.RegisterForImages(cam);
                faceDetector.OnFaceDetected -= m_onFaceDetectUpdate;
                faceDetector.OnFaceDetected += m_onFaceDetectUpdate;
            }
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            Logger.WriteLine("");
            if (!e.IsConfiguring && cam != null)
            {
                faceDetector.OnFaceDetected -= m_onFaceDetectUpdate;
                faceDetector.UnregisterForImages(cam);
                // de-register the trigger's faceDetection update handler                
                // try to shut down the camera           
                cam.TryStopFrameGrabbing();
            }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            Logger.WriteLine("OnSaved!");
            //   folderLocation = ((FaceDetectConfig)configurationControl).location;
            // assign selected cam
            cam = ((FaceDetectConfig)configurationControl).DeviceList.SelectedItem as Camera;

            // set the selected bounding rectangle
            boundingRect = ((MotionDetectorConfig)configurationControl).overlay.GetBoundingRect();
            Logger.WriteLine("BOUNDING RECT : " + boundingRect);
            triggerOnNrOfFaces = ((FaceDetectConfig)configurationControl).NumberOfFacesSelected;
        }
    }
}
