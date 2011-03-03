using System;
using System.Runtime.Serialization;
using System.Windows;
using MayhemCore;
using MayhemCore.ModuleTypes;
using VisionModules.Wpf;

namespace VisionModules
{
    [Serializable]
    public class MotionDetector : ActionBase, IWpf, ISerializable
    {
        
        public MotionDetector()
            : base("Motion Detector", "Detects when there is motion in the frame")
        {
            Setup();
        }
        
        public void Setup() {
          hasConfig = true;
          
          SetConfigString();
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
