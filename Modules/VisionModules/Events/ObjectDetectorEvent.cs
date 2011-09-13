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
using System.Linq;
using System.Text;
using MayhemCore;
using MayhemCore.ModuleTypes;
using Point = System.Drawing.Point;
using VisionModules.Wpf;
using MayhemOpenCVWrapper;
using MayhemOpenCVWrapper.LowLevel;
using MayhemDefaultStyles.UserControls;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Serialization;
using System.Windows;
using System.Collections;

namespace VisionModules.Events
{
    [DataContract]
    [MayhemModule("Object Detector", "Detects objects in scene matching a template image")]
    public class ObjectDetectorEvent : EventBase, IWpfConfigurable
    {
        private const int detectionInterval = 2500; //ms
        private DateTime lastObjectsDetected = DateTime.Now;
        private ObjectDetectorComponent od;
        private ObjectDetectorComponent.DetectionHandler objectDetectHandler;

        private CameraDriver i = CameraDriver.Instance;

        private Camera cam;

        // for some reason normal arrays are not serializable!
        [DataMember]
        Byte[] templateImage_bytes;

        // preview image for template --> shown in config dialog
        [DataMember]
        Byte[] templatePreview_bytes;

        [DataMember]
        string testString = "orig";

        public Bitmap templateImage;

        public Bitmap templatePreview;

        public double preview_scale_f = 1;

        [DataMember]
        Rect boundingRect = new Rect(0, 0, 0, 0);

        // the cam we have selected
        private int selected_device_idx = 0;

        public bool templateConfigured
        {
            get { return od.templateIsSet; }
            set { }
        }

        public ObjectDetectorEvent()
        {
            InitMe(new StreamingContext());
        }

        protected override void Initialize()
        {
            base.Initialize();
            //InitMe();
        }

        /// <summary>
        /// Called before (!!)  Deserialization / Instantiation
        /// </summary>
        /// 
        [OnDeserialized]
        public void InitMe(StreamingContext s)
        {
            if (i == null)
                i = CameraDriver.Instance;

            base.Initialize();
            if (selected_device_idx < i.devices_available.Length)
            {
                cam = i.cameras_available[selected_device_idx];
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
                od.set_template(templateImage);
            }

            if (templatePreview_bytes != null)
            {
                templatePreview = VisionModulesWPFCommon.ArrayToBitmap(templatePreview_bytes);
            }
            // -----------

            Logger.WriteLine(testString);
        }

        void m_onObjectDetected(object sender, List<Point> matchingKeyPoints)
        {
            // TODO
            TimeSpan ts = DateTime.Now - lastObjectsDetected;

            if (ts.TotalMilliseconds > detectionInterval)
            {

                Logger.WriteLine("m_OnMotionUpdate");

                // trigger the reaction
                base.OnEventActivated();

                lastObjectsDetected = DateTime.Now;
            }
        }

        public override void Enable()
        {
            base.Enable();
            Logger.WriteLine("Enable");

            // TODO: Improve this code
            if (selected_device_idx < i.devices_available.Length)
            {
                cam = i.cameras_available[selected_device_idx];
                if (cam.running == false)
                    cam.StartFrameGrabbing();

            }
            // register the trigger's motion update handler
            od.RegisterForImages(cam);
            od.OnObjectDetected += objectDetectHandler;
        }


        /** <summary>
         *  Passed from configuration dialog 
         * </summary>
         */
        public void setTemplateImage(Bitmap tImage)
        {
            od.set_template(tImage);
            templateImage = tImage;
        }

        public override void Disable()
        {
            base.Disable();
            Logger.WriteLine("Disable");

            // correct module disabling procedure
            od.UnregisterForImages(cam);
            od.OnObjectDetected -= objectDetectHandler;
            cam.TryStopFrameGrabbing();


        }

        protected new void SetConfigString()
        {
            ConfigString = String.Format("Configuration Message");
        }

        public IWpfConfiguration ConfigurationControl
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

                config.DeviceList.SelectedIndex = selected_device_idx;
                return config;
            }
        }

        public void OnSaved(IWpfConfiguration configurationControl)
        {
            bool wasEnabled = this.Enabled;

            if (this.Enabled)
                this.Disable();
            // assign selected cam
            // cam = ((CamSnapshotConfig)configurationControl).DeviceList.SelectedItem as Camera;

            if (wasEnabled)
                this.Enable();

            ObjectDetectorConfig config = configurationControl as ObjectDetectorConfig;

            cam = config.selected_camera;
            boundingRect = config.selectedBoundingRect;
            templateImage = config.templateImg;
            templatePreview = config.templatePreview;

            // save the bytes, too!

            templateImage_bytes = VisionModulesWPFCommon.BitmapToArray(templateImage);
            templatePreview_bytes = VisionModulesWPFCommon.BitmapToArray(templatePreview);

            // TODO: od.SetDetectionBoundaryRect(boundingRect) 

            testString = "FOOBAR";
        }
    }
}
