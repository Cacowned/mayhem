using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using MayhemCore.ModuleTypes;
using System.Runtime.Serialization;
using DefaultModules.WebcamHelpers;
using System.Drawing;
using DefaultModules.Wpf;
using System.Windows;
using System.IO;
using System.Drawing.Imaging;

namespace DefaultModules.Reactions
{
    [Serializable]
    public class WebcamSnapshot : ReactionBase, IWpf, ISerializable
    {
        protected string folderLocation = "";

        protected Webcam webcam;

        public WebcamSnapshot()
            : base("Webcam Snapshot", "Takes a photo with your webcam and saves it to the hard drive.") {

            
            Setup();
        }

        public void Setup() {
            webcam = Webcam.GetInstance();

            hasConfig = true;


            SetConfigString();
        }

        public override void Enable() {
            base.Enable();
            webcam.Start();
        }

        public override void Disable() {
            base.Disable();
            webcam.Stop();
        }

        public override void Perform() {
            
            

            Bitmap picture = webcam.GetFrame();
            
            string location = Path.Combine(folderLocation, DateTimeToTimeStamp(DateTime.Now))+".bmp";

            picture.Save(location, ImageFormat.Bmp);

//            imagefile = picture.Save()
            /*
            FileStream stream = File.OpenWrite(location);
            
            MessageBox.Show("Text","foo", MessageBoxButton.OK, )
            picture.Save(stream, ImageFormat.Bmp);
  //         stream.Write()

            //picture.Save(location, ImageFormat.Jpeg );
            
            // take snapshot
             */
        }

        protected string DateTimeToTimeStamp(DateTime time) {
            return time.ToString("yyyyMMddHHmmssffff");
        }

        public void WpfConfig() {
            /*
             * Choose a webcam and save folder
             */
            var window = new WebcamSnapshotConfig(folderLocation, webcam.CaptureDevice);
            
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;

            if (window.ShowDialog() == true) {

                folderLocation = window.location;
                webcam.CaptureDevice = window.captureDevice;

                SetConfigString();
            }
            // Choose camera
            
        }

        private void SetConfigString() {
            ConfigString = String.Format("Save Location: \"{0}\"", folderLocation);
        }

        #region Serialization
        public WebcamSnapshot(SerializationInfo info, StreamingContext context)
            : base(info, context) {

            folderLocation = info.GetString("FolderLocation");

            Setup();
        }

        public new void GetObjectData(SerializationInfo info, StreamingContext context) {
            base.GetObjectData(info, context);
            info.AddValue("FolderLocation", folderLocation);
        }
        #endregion
    }
}
