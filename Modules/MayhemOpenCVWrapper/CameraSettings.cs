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
        public int ResX
        {
            get;
            private set;
        }
        public int ResY
        {
            get;
            private set; 
        }
    
        public int UpdateRateMs
        {
            get;
            private set;
        }

        protected CameraSettings(int w, int h, int updateRate)
        {
            ResX = w;
            ResY = h;
            UpdateRateMs = updateRate; 
        }

        /// <summary>
        /// 320x240 resolution, 20 FPS update rate
        /// </summary>
        /// <returns></returns>
        public static CameraSettings Defaults320()
        {
            CameraSettings cs = new CameraSettings(320, 240, 50);
            return cs;
        }

        public static CameraSettings Defaults()
        {
            return CameraSettings.Defaults320();
        }

        /// <summary>
        /// 640x480 resolution, 20 FPS update rate
        /// </summary>
        /// <returns></returns>
        public static CameraSettings Defaults640()
        {
            CameraSettings cs = new CameraSettings(640, 480, 50);
            return cs;
        }
    }
}
