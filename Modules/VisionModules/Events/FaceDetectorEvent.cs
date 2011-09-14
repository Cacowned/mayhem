using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Windows;
using MayhemCore;
using MayhemCore.ModuleTypes;
using VisionModules.Wpf;
using MayhemOpenCVWrapper;
using System.Diagnostics;
using MayhemDefaultStyles.UserControls;
using MayhemOpenCVWrapper.LowLevel;

namespace VisionModules.Events
{
    [DataContract]
    [MayhemModuleAttribute("Face Detector", "Detects if and how many faces are in the scene")]
    public class FaceDetectorEvent : EventBase, IWpfConfigurable
    {
        private const int detectionInterval = 2500; //ms
        private DateTime lastFacesDetected = DateTime.Now;
        private FaceDetectorComponent fd;
        private FaceDetectorComponent.DetectionHandler faceDetectUpdateHandler;

        private CameraDriver i = CameraDriver.Instance;

        private Camera cam = null;

        // the cam we have selected
        [DataMember]
        private int selected_device_idx = 0;

        [DataMember]
        Rect boundingRect; 

        public FaceDetectorEvent()
        {
            Initialize();
        }
      
         /** <summary>
         * Called when deserialized / on instantiation
         * </summary> */
        protected override void Initialize()
        {
            if (i == null)
                i = CameraDriver.Instance; 

            if (selected_device_idx < i.devices_available.Length)
            {
                cam = i.cameras_available[selected_device_idx];
            }
            else
            {
                Logger.WriteLine("No camera available");
            }

            fd = new FaceDetectorComponent();
            faceDetectUpdateHandler = new FaceDetectorComponent.DetectionHandler(m_onFaceDetected);

            // TODO
            /*
            if (boundingRect.Width > 0 && boundingRect.Height > 0)
            {
                m.SetMotionBoundaryRect(boundingRect);
            } */
        }

        void m_onFaceDetected(object sender, List<System.Drawing.Point> points)
        {
            // number of faces is points.Size() / 4
            TimeSpan ts = DateTime.Now - lastFacesDetected;
            if (points.Count > 0 && ts.TotalMilliseconds > detectionInterval)
            {
                Logger.WriteLine("m_onFaceDetected");
                base.OnEventActivated();

                lastFacesDetected = DateTime.Now;
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
                Logger.WriteLine(" get ConfigurationControl!");

                // TODO
                //string folderLocation = "";
                FaceDetectConfig config = new FaceDetectConfig();
                if (boundingRect.Width > 0 && boundingRect.Height > 0)
                {            
                    config.selectedBoundingRect = boundingRect;
                }

                config.DeviceList.SelectedIndex = selected_device_idx;

                return config;
            }
        }

        public override void Enable()
        {
            base.Enable();
            Logger.WriteLine("EnableTrigger");

            // TODO: Improve this code
            if (selected_device_idx < i.devices_available.Length)
            {
                cam = i.cameras_available[selected_device_idx];
                if (cam.running == false)
                    cam.StartFrameGrabbing();

            }
            // register the trigger's faceDetection update handler
            fd.RegisterForImages(cam);
            fd.OnFaceDetected += m_onFaceDetected;
        }

        public override void Disable()
        {
            base.Disable();
            Logger.WriteLine("DisableTrigger");
            // de-register the trigger's faceDetection update handler
            fd.UnregisterForImages(cam);
            fd.OnFaceDetected -= m_onFaceDetected;
            // try to shut down the camera
            cam.TryStopFrameGrabbing();
        }

        public  void OnSaved(IWpfConfiguration configurationControl)
        {
            Logger.WriteLine("OnSaved!");
      
            //   folderLocation = ((FaceDetectConfig)configurationControl).location;

            bool wasEnabled = this.Enabled;

            if (this.Enabled)
                this.Disable();
            // assign selected cam
            cam = ((FaceDetectConfig)configurationControl).DeviceList.SelectedItem as Camera;

            // set the selected bounding rectangle
            boundingRect = ((MotionDetectorConfig)configurationControl).overlay.GetBoundingRect();

            Logger.WriteLine("BOUNDING RECT : " + boundingRect); 
            
            // TODO: m.SetMotionBoundaryRect(boundingRect); 

            if (wasEnabled)
                this.Enable();
        }
    }
}
