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
        private DateTime lastFacesDetected = DateTime.Now;
        private FaceDetectorComponent fd;
        private FaceDetectorComponent.DetectionHandler faceDetectUpdateHandler;

        private CameraDriver i = CameraDriver.Instance;

        private Camera cam = null;

        // the cam we have selected
        [DataMember]
        private int selected_device_idx = 0;

        [DataMember]
        private Rect boundingRect; 

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

            if (selected_device_idx < i.DeviceCount)
            {
                cam = i.cameras_available[selected_device_idx];
            }
            else
            {
                Logger.WriteLine("No camera available");
            }

            fd = new FaceDetectorComponent();
            faceDetectUpdateHandler = new FaceDetectorComponent.DetectionHandler(m_onFaceDetected);
        }

        void m_onFaceDetected(object sender, List<System.Drawing.Point> points)
        {
            // number of faces is points.Size() / 4
            TimeSpan ts = DateTime.Now - lastFacesDetected;
            if (points.Count > 0 && ts.TotalMilliseconds > detectionInterval)
            {
                Logger.WriteLine("m_onFaceDetected");
                base.Trigger();

                lastFacesDetected = DateTime.Now;
            }
        }

        protected new void SetConfigString()
        {
            ConfigString = String.Format("Configuration Message");
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

        public override bool Enable()
        {
            // TODO: Improve this code
            if (!IsConfiguring && selected_device_idx < i.DeviceCount)
            {
                cam = i.cameras_available[selected_device_idx];
                //if (cam.running == false)
                Thread.Sleep(350);
                cam.StartFrameGrabbing();
                // register the trigger's faceDetection update handler
                fd.RegisterForImages(cam);
                fd.OnFaceDetected -= m_onFaceDetected;
                fd.OnFaceDetected += m_onFaceDetected;
            }
            return true;
        }

        public override void Disable()
        {
            Logger.WriteLine("");       
            if (!IsConfiguring && cam != null)
            {
                fd.OnFaceDetected -= m_onFaceDetected;
                fd.UnregisterForImages(cam); 
                // de-register the trigger's faceDetection update handler                
                // try to shut down the camera           
                cam.TryStopFrameGrabbing();              
            }
        }

        public  void OnSaved(WpfConfiguration configurationControl)
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
