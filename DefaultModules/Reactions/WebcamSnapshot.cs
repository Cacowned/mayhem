using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using MayhemCore.ModuleTypes;
using System.Runtime.Serialization;
using DefaultModules.WebcamHelpers;

namespace DefaultModules.Reactions
{
    [Serializable]
    public class WebcamSnapshot : ReactionBase, IWpf, ISerializable
    {
        protected string folderLocation = "";

        Webcam camera;

        public WebcamSnapshot()
            : base("Webcam Snapshot", "Takes a photo with your webcam and saves it to the hard drive.") {

            
            Setup();

        }

        public void Setup() {
            hasConfig = true;

            

            SetConfigString();
        }

        public override void Perform() {

            // take snapshot
        }

        public void WpfConfig() {
            /*
             * Choose a webcam and save folder
             */
            
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
