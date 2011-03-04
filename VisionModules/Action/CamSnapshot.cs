using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using MayhemCore.ModuleTypes;
using System.Runtime.Serialization;
using System.Windows;
using VisionModules.Wpf;

namespace VisionModules.Action
{
    [Serializable]
    public class CamSnapshot : ReactionBase, IWpf, ISerializable
    {
        protected string folderLocation = "";

        // The device we are recording from
        

        public CamSnapshot()
            : base("Webcam Snapshot (OpenCV)", "Takes a photo with your webcam and saves it to the hard drive.")
        {


            Setup();
        }

        public void Setup()
        {

            hasConfig = true;
            // TODO: What if we have multiple of these?
           
            SetConfigString();
        }

        public override void Enable()
        {
            base.Enable();

           // TODO: Start Cam
        }

        public override void Disable()
        {
            base.Disable();
           
            // TODO: Stop Cam
        }

        public override void Perform()
        {
            // Executes Action

           
        }

        protected string DateTimeToTimeStamp(DateTime time)
        {
            return time.ToString("yyyyMMddHHmmssffff");
        }

        public void WpfConfig()
        {
            /*
             * Choose a webcam and save folder
             */
            var window = new CamSnapshotConfig(folderLocation, null /* cameraDevice */);

            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            if (window.ShowDialog() == true)
            {

                folderLocation = window.location;

                // webcam.Device = window.captureDevice;

                SetConfigString();
            }

        }

        private void SetConfigString()
        {
            ConfigString = String.Format("Save Location: \"{0}\"", folderLocation);
        }

        #region Serialization
        public CamSnapshot(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

            folderLocation = info.GetString("FolderLocation");

            Setup();
        }

        public new void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("FolderLocation", folderLocation);
        }
        #endregion
    }
}
