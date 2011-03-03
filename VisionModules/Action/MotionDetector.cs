using System;
using System.Runtime.Serialization;
using System.Windows;
using MayhemCore;
using MayhemCore.ModuleTypes;
using VisionModules.Wpf;
using MayhemOpenCVWrapper;
using System.Collections.Generic;
using System.Diagnostics;

namespace VisionModules
{
    [Serializable]
    public class MotionDetector : ActionBase, IWpf, ISerializable
    {

        private const string TAG = "[MotionDetector (Action)] : ";
        private DateTime lastMotionDetected = DateTime.Now;
        private const int detectionInterval = 5000; //ms

        private MayhemOpenCVWrapper.MotionDetector m;
        Rect boundingRect = new Rect();

        private MayhemOpenCVWrapper.MotionDetector.MotionUpdateHandler motionUpdateHandler;

        private MayhemImageUpdater i = MayhemImageUpdater.Instance;

        
        public MotionDetector()
            : base("Motion Detector", "Detects when there is motion in the frame")
        {

            Setup();
        }
        
        public void Setup() {

            if (i.selected_device == null && i.devices_available.Length > 0)
            {
                // use the first device for now
                // TODO: Camera framework needs to be initialized globally 
                i.InitCaptureDevice(0, 320, 240);
            }

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

               // trigger the readction
                OnActionActivated();

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
            
            if (window.ShowDialog() == true) {

                // Grab data from the window variable and store
                // it in this class
                
                SetConfigString();
            }
        }

        public override void Enable() {
            base.Enable();
        }

        public override void Disable() {
            base.Disable();
        }

        #region Serialization

        public MotionDetector(SerializationInfo info, StreamingContext context) 
            : base (info, context)
        {
            // Code goes here
            Setup();
        }

        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            // code goes here
        }
        #endregion
    }
}
