using System;
using System.Runtime.Serialization;
using System.Windows;
using MayhemCore;
using MayhemCore.ModuleTypes;
using VisionModules.Wpf;
using MayhemOpenCVWrapper;
using System.Collections.Generic;
using System.Diagnostics;

namespace VisionModules.Actions
{
    [Serializable]
    public class MotionDetectorAction : ActionBase, IWpf, ISerializable
    {

        private const string TAG = "[MotionDetectorAction] : ";
        private DateTime lastMotionDetected = DateTime.Now;
        private const int detectionInterval = 5000; //ms

        private MayhemOpenCVWrapper.MotionDetector m;
        Rect boundingRect = new Rect();

        private MayhemOpenCVWrapper.MotionDetector.MotionUpdateHandler motionUpdateHandler;

        private MayhemCameraDriver i = MayhemCameraDriver.Instance;
        private Camera cam = null;

        // which cam have we selected
        private int selected_device_idx = 0; 
        
        public MotionDetectorAction()
            : base("Motion Detector", "Detects when there is motion in the frame")
        {

            Setup();
        }
        
        public void Setup() {

          
            /*
            if (i.cameras_available.Length > 0)
            {
                cam = i.cameras_available[0];
                if (!cam.running) cam.StartFrameGrabbing();
            }
            else
            {
                Debug.WriteLine(TAG + "No camera available");
            } */

            m = new MayhemOpenCVWrapper.MotionDetector(320, 240);
            motionUpdateHandler = new MayhemOpenCVWrapper.MotionDetector.MotionUpdateHandler(m_OnMotionUpdate);

          hasConfig = true;
          
          SetConfigString();
        }


        /**<summary>
         * Activates "Action" action when motion is detected. 
         * </summary>
         * 
         */ 
        void m_OnMotionUpdate(object sender, List<System.Drawing.Point> points)
        {

            TimeSpan ts = DateTime.Now - lastMotionDetected;

            if (ts.TotalMilliseconds > detectionInterval)
            {

                Debug.WriteLine(TAG + "m_OnMotionUpdate");

               // trigger the reaction
                base.OnActionActivated();

                lastMotionDetected = DateTime.Now;
            }
        }


        protected void SetConfigString()
        {
            ConfigString = String.Format("Configuration Message");
        }

        public void WpfConfig() {
            var window = new MotionDetectorConfig(); // pass the parameters to initially populate the window in the constructor
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            window.DeviceList.SelectedIndex = selected_device_idx;
            
            if (window.ShowDialog() == true) {

                // Grab data from the window variable and store
                // it in this class
                bool wasEnabled = this.Enabled;
               
                if (this.Enabled) this.Disable();
                // assign selected cam
                cam = window.DeviceList.SelectedItem as Camera;

                if (wasEnabled) this.Enable();
               
                SetConfigString();
            }
        }

        public override void Enable() {
            base.Enable();
            Debug.WriteLine(TAG + "EnableTrigger");

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

        public override void Disable() {
            base.Disable();
            Debug.WriteLine(TAG + "DisableTrigger");
            // de-register the trigger's motion update handler
        
            m.OnMotionUpdate -= motionUpdateHandler;
        }

        #region Serialization

        public MotionDetectorAction(SerializationInfo info, StreamingContext context) 
            : base (info, context)
        {
            // Code goes here

            string camera_description = "";

            // try to initialize the camera from the camera ID
            try
            {
                selected_device_idx = info.GetInt32("CameraID");
                camera_description = info.GetString("CameraName");
            }
            catch (Exception ex)
            {
                selected_device_idx = 0;

            }

            // see if the particular cam is still present

            if (selected_device_idx < i.cameras_available.Length)
            {
                if (camera_description.Equals(i.cameras_available[selected_device_idx].info.description))
                {
                    // great!, do nothing
                }
                else
                {
                    // default cam
                    selected_device_idx = 0;
                }
            }

            Setup();

        }

        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            // code goes here
            info.AddValue("CameraID", (Int32)cam.info.deviceId);
            info.AddValue("CameraName", cam.info.description);


        }
        #endregion
    }
}
