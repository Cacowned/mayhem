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
    /// Contains settings for cameras
    /// </summary>
    public class CameraSettings
    {
        /// <summary>
        /// camera resolution in X dimension (i.e. image width)
        /// </summary>
        public int ResX
        {
            get;
            private set;
        }

        /// <summary>
        /// camera resolution in Y dimension (i.e. image height)
        /// </summary>
        public int ResY
        {
            get;
            private set; 
        }
    
        /// <summary>
        /// camera update rate in milliseconds
        /// </summary>
        public int UpdateRateMs
        {
            get;
            private set;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="w">image width</param>
        /// <param name="h">image height</param>
        /// <param name="updateRate">update rate in milliseconds</param>
        private CameraSettings(int w, int h, int updateRate)
        {
            ResX = w;
            ResY = h;
            UpdateRateMs = updateRate; 
        }

        /// <summary>
        /// Retrieves the default (640x480, 20fps) camera settings for use in Mayhem
        /// </summary>
        /// <returns></returns>
        public static CameraSettings Defaults()
        {
            return CameraSettings.Defaults640();
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
