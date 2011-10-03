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
    /// Contains settings for  cameras
    /// TODO: find out what more can be set
    /// </summary>
    public class CameraSettings
    {
        public int resX
        {
            get;
            private set;
        }
        public int resY
        {
            get;
            private set; 
        }
    
        public int updateRate_ms
        {
            get;
            private set;
        }

        protected CameraSettings(int w, int h, int updateRate)
        {
            resX = w;
            resY = h;
            updateRate_ms = updateRate; 
        }

        /// <summary>
        /// 320x240 resolution, 20 FPS update rate
        /// </summary>
        /// <returns></returns>
        public static CameraSettings DEFAULTS_320()
        {
            CameraSettings cs = new CameraSettings(320, 240, 50);
            return cs;
        }

        public static CameraSettings DEFAULTS()
        {
            return CameraSettings.DEFAULTS_320();
        }

        /// <summary>
        /// 640x480 resolution, 20 FPS update rate
        /// </summary>
        /// <returns></returns>
        public static CameraSettings DEFAULTS_640()
        {
            CameraSettings cs = new CameraSettings(640, 480, 50);
            return cs;
        }
    }
}
