using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    /// <summary>
    /// A basic Face Detector Event:
    /// face detector now only triggers when the amount of faces surpasses a threshold value, and only once then. 
    /// When the amount of faces goes below the threshold and then above it again, the reaction triggers again, and so forth. 
    /// </summary>
    [DataContract]
    [MayhemModule("Face Detector", "Detects if and how many faces are in the scene")]
    public class FaceDetectorEvent : EventBase, IWpfConfigurable
    {
        // ms
        private const int DetectionInterval = 2500; 

        // the cam we have selected
        [DataMember]
        private int selectedDeviceIndex;

        [DataMember]
        private Rect boundingRect;

        [DataMember]
        private int triggerOnNrOfFaces;

        private DateTime lastFacesDetectedTime = DateTime.Now;
        private FaceDetectorComponent faceDetector;
        private CameraDriver cameraDriver;
        private ImagerBase camera;
        private int lastFacesDetectedAmount;

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
                camera = cameraDriver.CamerasAvailable[selectedDeviceIndex];
                faceDetector = new FaceDetectorComponent(camera);
            }
            else
            {
                Logger.WriteLine("No camera available");
                ErrorLog.AddError(ErrorType.Warning, "FaceDetector is disabled because no camera was detected");
                camera = null;
                
            }

           
        }

        private void OnFaceDetectUpdate(object sender, DetectionEventArgs pts)
        {
            List<System.Drawing.Point> points = pts.Points;

            // number of faces is points.Size() / 4
            // as each faces is returned with its own bounding box
            int nrFacesDetected = points.Count / 2;
            Logger.WriteLine("m_onFaceDetected: count " + nrFacesDetected);
            TimeSpan ts = DateTime.Now - lastFacesDetectedTime;

            if (points.Count > 0 && ts.TotalMilliseconds > DetectionInterval)
            {
                // only trigger if the lastFacesDetectedAmount was smaller than the trigger threshold
                // and only if the number of faces detected this time is greater or equal then the trigger threshold
                if (lastFacesDetectedAmount < triggerOnNrOfFaces && nrFacesDetected >= triggerOnNrOfFaces)
                {
                    Trigger();
                }

                lastFacesDetectedTime = DateTime.Now;
            }

            lastFacesDetectedAmount = nrFacesDetected;
        }

        public string GetConfigString()
        {
            string config = string.Empty;
            if (camera != null)
            {
                config += "Camera: " + camera.Info.DeviceId + ", ";
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
                FaceDetectConfig config = new FaceDetectConfig(camera as Camera);
                if (boundingRect.Width > 0 && boundingRect.Height > 0)
                {
                    config.SelectedBoundingRect = boundingRect;
                }

                config.DeviceList.SelectedIndex = selectedDeviceIndex;
                return config;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected override void OnEnabling(EnablingEventArgs e)
        {
            if (!e.WasConfiguring)
            {
                if (selectedDeviceIndex < cameraDriver.DeviceCount)
                {
                    camera = cameraDriver.CamerasAvailable[selectedDeviceIndex];
                }
                else if (cameraDriver.DeviceCount > 0)
                {
                    camera = cameraDriver.CamerasAvailable[0];
                }
                else
                {
                    camera = null;
                }

                if (camera != null)
                {
                    if (!camera.Running)
                        camera.StartFrameGrabbing();
                    // register the trigger's faceDetection update handler
                    faceDetector.RegisterForImages(camera);
                    faceDetector.OnFaceDetected -= OnFaceDetectUpdate;
                    faceDetector.OnFaceDetected += OnFaceDetectUpdate;
                }
            }
            else if (camera == null)
            {
                Logger.WriteLine("No camera available");
                ErrorLog.AddError(ErrorType.Warning, "FaceDetector is disabled because no camera was detected");
                throw new NotSupportedException("No Camera");
            }              
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected override void OnDisabled(DisabledEventArgs e)
        {
            if (!e.IsConfiguring && camera != null)
            {
                faceDetector.OnFaceDetected -= OnFaceDetectUpdate;
                faceDetector.UnregisterForImages(camera);

                // de-register the trigger's faceDetection update handler                
                // try to shut down the camera           
                camera.TryStopFrameGrabbing();
            }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            // assign selected cam
            var config = configurationControl as FaceDetectConfig;

            camera = config.DeviceList.SelectedItem as Camera;

            // set the selected bounding rectangle
            boundingRect = config.overlay.GetBoundingRect();
            Logger.WriteLine("BOUNDING RECT : " + boundingRect);
            triggerOnNrOfFaces = config.NumberOfFacesSelected;
        }
    }
}
