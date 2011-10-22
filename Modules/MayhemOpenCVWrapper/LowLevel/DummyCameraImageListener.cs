using System;

namespace MayhemOpenCVWrapper.LowLevel
{
    /// <summary>
    /// A dummy class acting as a camera listener. 
    /// Fake Camera Listener used by Picture and Vidoe to keep subscribed to camera updates
    /// </summary>
    public class DummyCameraImageListener : CameraImageListener
    {
        public override void UpdateFrame(object sender, EventArgs e)
        {
           // Do Nothing
        }
    }
}
