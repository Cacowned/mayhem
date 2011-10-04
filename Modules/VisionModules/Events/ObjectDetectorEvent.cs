/*
 *  ObjectDetectorEvent.cs
 * 
 *  Object Detector Mayhem Module
 * 
 * (c) Microsoft Applied Sciences Group, 2011
 * 
 *  Author: Sven Kratz
 * 
 */

// TODO: Handle serialization/deserialization of template image !!!!!!

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows;
using MayhemCore;
using MayhemOpenCVWrapper;
using MayhemOpenCVWrapper.LowLevel;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using VisionModules.Wpf;
using Point = System.Drawing.Point;

namespace VisionModules.Events
{
    [DataContract]
    [MayhemModule("Object Detector", "Detects objects in scene matching a template image")]
    public class ObjectDetectorEvent : EventBase, IWpfConfigurable
    {
        [DataMember]
        private Byte[] templateImage_bytes;

        // preview image for template --> shown in config dialog
        [DataMember]
        private Byte[] templatePreview_bytes;

        [DataMember]
        private Rect boundingRect;

        private const int detectionInterval = 2500; //ms
        private DateTime lastObjectsDetected = DateTime.Now;
        private ObjectDetectorComponent od;
        private ObjectDetectorComponent.DetectionHandler objectDetectHandler;

        private CameraDriver cameraDriver = CameraDriver.Instance;

        private Camera camera;

        public Bitmap templateImage;

        public Bitmap templatePreview;

        public double previewScaleF = 1;

        // the cam we have selected
        private int selectedDeviceIndex = 0;

        public bool templateConfigured
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
            objectDetectHandler = new ObjectDetectorComponent.DetectionHandler(m_onObjectDetected);

            // ----------- deserialize image and image preview
            if (templateImage_bytes != null)
            {
                templateImage = VisionModulesWPFCommon.ArrayToBitmap(templateImage_bytes);
                od.SetTemplate(templateImage);
            }

            if (templatePreview_bytes != null)
            {
                templatePreview = VisionModulesWPFCommon.ArrayToBitmap(templatePreview_bytes);
            }
        }

        void m_onObjectDetected(object sender, List<Point> matchingKeyPoints)
        {
            // TODO
            TimeSpan ts = DateTime.Now - lastObjectsDetected;

            if (ts.TotalMilliseconds > detectionInterval)
            {

                Logger.WriteLine("m_OnMotionUpdate");

                // trigger the reaction
                base.Trigger();

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

        /** <summary>
         *  Passed from configuration dialog 
         * </summary>
         */
        public void setTemplateImage(Bitmap tImage)
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
            // TODO
            //get { throw new NotImplementedException(); }
            get
            {
                // TODO (!!!!) 
                //string folderLocation = "";

                // For now use the FaceDetect Config, and hardcode the template image 
                ObjectDetectorConfig config = new ObjectDetectorConfig(this, null);

                if (boundingRect.Width > 0 && boundingRect.Height > 0)
                {
                    config.selectedBoundingRect = boundingRect;
                }

                if (templateImage != null)
                    config.templateImg = templateImage;


                if (templatePreview != null)
                    config.templatePreview = templatePreview;

                config.DeviceList.SelectedIndex = selectedDeviceIndex;
                return config;
            }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            bool wasEnabled = this.IsEnabled;

            // assign selected cam
            // cam = ((CamSnapshotConfig)configurationControl).DeviceList.SelectedItem as Camera;

            ObjectDetectorConfig config = configurationControl as ObjectDetectorConfig;

            camera = config.selected_camera;
            boundingRect = config.selectedBoundingRect;
            templateImage = config.templateImg;
            templatePreview = config.templatePreview;

            // save the bytes, too!

            templateImage_bytes = VisionModulesWPFCommon.BitmapToArray(templateImage);
            templatePreview_bytes = VisionModulesWPFCommon.BitmapToArray(templatePreview);

            // TODO: od.SetDetectionBoundaryRect(boundingRect) 
        }
    }
}
