/*
 * CameraSettings.cs
 * 
 * Storage Class for camera initialization settings.
 * 
 * (c) 2010/2011, Microsoft Applied Sciences Group
 * 
 * Author: Sven Kratz
 * 
 */


namespace MayhemOpenCVWrapper
{
    /// <summary>
    /// Contians settings for  cameras
    /// TODO: find out what more can be set
    /// </summary>
    public struct CameraSettings
    {
        public int resX;
        public int resY;
        // bpp? 
        // refresh rate ?
        // pixel format ?
        private int updateRate_ms_;

        public int updateRate_ms
        {
            get
            {
                return updateRate_ms_;
            }
        }

        public static CameraSettings DEFAULTS()
        {
            CameraSettings cs;
            cs.resX = 320;
            cs.resY = 240;
            cs.updateRate_ms_ = 50;    // 20 fps
            return cs;
        }

    }
}
