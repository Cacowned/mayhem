// TODO: Handle serialization / deserialization of template image !!!!!!

using System;
using System.Drawing;
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
    /// Still under development!
    /// 
    /// Object Detector Mayhem Module
    /// </summary>
    [DataContract]
    [MayhemModule("Object Detector", "Detects objects in scene matching a template image")]
    internal class ObjectDetectorEvent : EventBase, IWpfConfigurable
    {
        // ms
        private const int DetectionInterval = 2500; 

        [DataMember]
        private byte[] templateImageBytes;

        // preview image for template --> shown in config dialog
        [DataMember]
        private byte[] templatePreviewBytes;

        [DataMember]
        private Rect boundingRect;

        private DateTime lastObjectsDetected = DateTime.Now;
        private ObjectDetectorComponent od;
        private ObjectDetectorComponent.DetectionHandler objectDetectHandler;

        private CameraDriver cameraDriver = CameraDriver.Instance;

        private Camera camera;

        private Bitmap templateImage;

        private Bitmap templatePreview;

        // the cam we have selected
        private int selectedDeviceIndex;

        public bool TemplateConfigured
        {
            get { return od.TemplateIsSet; }
        }

        protected override void OnLoadDefaults()
        {
            boundingRect = new Rect(0, 0, 0, 0);
        }

        protected override void OnAfterLoad()
        {
            cameraDriver = CameraDriver.Instance;

            if (selectedDeviceIndex < cameraDriver.DeviceCount)
            {
                camera = cameraDriver.CamerasAvailable[selectedDeviceIndex];
            }
            else
            {
                Logger.WriteLine("No camera available");
            }

            od = new ObjectDetectorComponent(320, 240);
            objectDetectHandler = OnObjectDetected;

            // ----------- deserialize image and image preview
            if (templateImageBytes != null)
            {
                templateImage = VisionModulesWpfCommon.ArrayToBitmap(templateImageBytes);
                od.SetTemplate(templateImage);
            }

            if (templatePreviewBytes != null)
            {
                templatePreview = VisionModulesWpfCommon.ArrayToBitmap(templatePreviewBytes);
            }
        }

        private void OnObjectDetected(object sender, DetectionEventArgs e)
        {
            // TODO
            TimeSpan ts = DateTime.Now - lastObjectsDetected;

            if (ts.TotalMilliseconds > DetectionInterval)
            {
                // trigger the reaction
                Trigger();

                lastObjectsDetected = DateTime.Now;
            }
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
            Logger.WriteLine("Enable");

            // TODO: Improve this code
            if (!e.WasConfiguring && selectedDeviceIndex < cameraDriver.DeviceCount)
            {
                camera = cameraDriver.CamerasAvailable[selectedDeviceIndex];
                if (camera.Running == false)
                    camera.StartFrameGrabbing();

                // register the trigger's motion update handler
                od.RegisterForImages(camera);
                od.OnObjectDetected += objectDetectHandler;
                od.OnObjectDetected -= objectDetectHandler;
            }
        }

        /// <summary>
        /// Passed from configuration dialog 
        /// </summary>
        /// <param name="tImage"></param>
        public void SetTemplateImage(Bitmap tImage)
        {
            od.SetTemplate(tImage);
            templateImage = tImage;
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            Logger.WriteLine("Disable");

            od.OnObjectDetected -= objectDetectHandler;
            if (camera != null)
                od.UnregisterForImages(camera);

            if (camera != null && !e.IsConfiguring)
            {
                // correct module disabling procedure               
                camera.TryStopFrameGrabbing();
            }
        }

        public string GetConfigString()
        {
            return "Configuration Message";
        }

        public WpfConfiguration ConfigurationControl
        {
            get
            {
                // TODO (!!!!) 

                // For now use the FaceDetect Config, and hardcode the template image 
                ObjectDetectorConfig config = new ObjectDetectorConfig();

                if (boundingRect.Width > 0 && boundingRect.Height > 0)
                {
                    config.SelectedBoundingRect = boundingRect;
                }

                if (templateImage != null)
                    config.TemplateImg = templateImage;

                if (templatePreview != null)
                    config.TemplatePreview = templatePreview;

                config.DeviceList.SelectedIndex = selectedDeviceIndex;
                return config;
            }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            ObjectDetectorConfig config = configurationControl as ObjectDetectorConfig;

            camera = config.SelectedCamera;
            boundingRect = config.SelectedBoundingRect;
            templateImage = config.TemplateImg;
            templatePreview = config.TemplatePreview;

            // save the bytes, too!
            templateImageBytes = VisionModulesWpfCommon.BitmapToArray(templateImage);
            templatePreviewBytes = VisionModulesWpfCommon.BitmapToArray(templatePreview);

            // TODO: od.SetDetectionBoundaryRect(boundingRect) 
        }
    }
}
