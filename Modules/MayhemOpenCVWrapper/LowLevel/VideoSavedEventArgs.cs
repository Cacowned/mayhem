using System;

namespace MayhemOpenCVWrapper.LowLevel
{
    public class VideoSavedEventArgs : EventArgs
    {
        public bool SavedSuccessfully
        {
            get;
            private set;
        }

        public VideoSavedEventArgs(bool success) 
        {
            SavedSuccessfully = success;
        }
    }
}
