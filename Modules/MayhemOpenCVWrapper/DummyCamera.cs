using System.Drawing;
using MayhemCore;

namespace MayhemOpenCVWrapper
{
    /// <summary>
    /// Dummy Camera object (useful for debugging) 
    /// </summary>
    public class DummyCamera : ImagerBase
    {
        public override event ImageUpdateHandler OnImageUpdated;

        public override CameraInfo Info
        {
            get { return CameraInfo.DummyInfo(); }
            protected set { }
        }

        public override CameraSettings Settings
        {
            get { return CameraSettings.Defaults(); }
            protected set { }
        }

        public override bool Running
        {
            get;
            protected set;
        }

        public override Bitmap ImageAsBitmap()
        {
            Logger.WriteLine("returning dummy image");
            
            // TODO: add Mayhem's logo or something 
            return new Bitmap(Settings.ResX, Settings.ResY);
        }

        public override void StartFrameGrabbing()
        {
            // no-op
            Logger.WriteLine("StartFrameGrabbing");
        }

        public override bool TryStopFrameGrabbing()
        {
            Logger.WriteLine("TryStopFrameGrabbing");
            return true;
        }
    }
}
