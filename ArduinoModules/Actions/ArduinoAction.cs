using System;
using System.Runtime.Serialization;
using System.Windows;
using MayhemCore;
using MayhemCore.ModuleTypes;
using System.Collections.Generic;
using System.Diagnostics;

namespace ArduinoModules.Actions
{
    [Serializable]
    public class MotionDetectorAction : ActionBase, IWpf, ISerializable
    {

        private const string TAG = "[ArdiunoAction] : ";
    
        private int selected_device_idx = 0; 
        
        public MotionDetectorAction()
            : base("Arduino Action", "Monitors pins on an Arduino")
        {

            Setup();
        }
        
        public void Setup() {

            // todo

       
        }


     


        protected void SetConfigString()
        {
            ConfigString = String.Format("Configuration Message");
        }

        public void WpfConfig() {
            /* TODO
            var window = new MotionDetectorConfig(); // pass the parameters to initially populate the window in the constructor
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            window.DeviceList.SelectedIndex = selected_device_idx;
            
            if (window.ShowDialog() == true) {

                // Grab data from the window variable and store
                // it in this class
                bool wasEnabled = this.Enabled;
               
                if (this.Enabled) this.Disable();
                // assign selected cam
                cam = window.selected_camera;

                if (wasEnabled) this.Enable();
               
                SetConfigString();
            }*/
        }

        public override void Enable() {
            base.Enable();
            Debug.WriteLine(TAG + "EnableTrigger");

           

        }

        public override void Disable() {
            base.Disable();
            Debug.WriteLine(TAG + "DisableTrigger");
            // de-register the trigger's motion update handler
            
        }

        #region Serialization

        public MotionDetectorAction(SerializationInfo info, StreamingContext context) 
            : base (info, context)
        {
            // TODO

            Setup();

        }

        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            
            // TODO
           


        }
        #endregion
    }
}
