using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Windows;
using MayhemCore;
using MayhemOpenCVWrapper;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using VisionModules.Events.Components;
using VisionModules.Wpf;

namespace VisionModules.Events
{
    /// <summary>
    /// Motion Detector Mayhem Module
    /// </summary>
    [DataContract]
    [MayhemModule("Motion Detector", "Detects when there is motion in the frame")]
    public class MotionDetector : EventBase, IWpfConfigurable
    {
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
            else
            {
                Logger.WriteLine("No camera available");
                ErrorLog.AddError(ErrorType.Warning, "MotionDector is disabled because no camera was detected");
                camera = null;
            }

            
        }

        private void OnMotionUpdated(object sender, EventArgs e)
        {
            if (!firstFrame)
                Trigger();
                
            firstFrame = false;
        }

        public string GetConfigString()
        {
            string conf = string.Empty; 
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
                    config.SelectedBoundingRect = boundingRect; 
                }

                config.Sensitivity = sensitivity;

                return config;
            }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            // set the selected bounding rectangle
            boundingRect = ((MotionDetectorConfig)configurationControl).SelectedBoundingRect;
            motionDetectorComponent.SetMotionBoundaryRect(boundingRect);

            // assign selected cam
            camera = ((MotionDetectorConfig)configurationControl).SelectedCamera;

            motionDetectorComponent.Sensitivity = ((MotionDetectorConfig)configurationControl).Sensitivity;
            sensitivity = motionDetectorComponent.Sensitivity;

            selectedDeviceIndex = camera.Info.DeviceId;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected override void OnEnabling(EnablingEventArgs e)
        {           
            Logger.WriteLine("Enable");

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
                    firstFrame = true;

                    // register the trigger's motion update handler
                    motionDetectorComponent.RegisterForImages(camera);
                    motionDetectorComponent.OnMotionUpdate -= OnMotionUpdated;
                    motionDetectorComponent.OnMotionUpdate += OnMotionUpdated;        
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
