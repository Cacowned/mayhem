/*
 * CameraInfo.cs
 * 
 * Storage class for info about camera capabilities
 * 
 * (c) 2010/2011, Microsoft Applied Sciences Group
 * 
 * Author: Sven Kratz
 * 
 */

namespace MayhemOpenCVWrapper
{
    /** <summary>Just a wrapper for information about detected cameras 
       could in future be extended to contain the camera's capabilities (i.e. max resolutions, etc.)
        </summary>
     * */
    public class CameraInfo
    {
        /// <summary>
        /// position of the camera in the list of cameras visible to Mayhem
        /// </summary>
        public int DeviceId
        {
            get;
            private set; 
        }

        /// <summary>
        /// text description of the camera or the camera name.  
        /// </summary>
        public string Description
        {
            get;
            private set; 
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="id">position of the camera in the list of cameras visible to Mayhem</param>
        /// <param name="descr">text description of the camera or the camera name</param>
        public CameraInfo(int id, string descr)
        {
            DeviceId = id;
            Description = descr;
        }

        /// <summary>
        /// Fake camera information (for use with the Dummy Camera) 
        /// </summary>
        /// <returns>CameraInfo object</returns>
        public static CameraInfo DummyInfo()
        {
            return new CameraInfo(0, "Dummmy Camera");
        }

        /// <summary>
        /// String representation of the CameraInfo object 
        /// </summary>
        /// <returns>String</returns>
        public override string ToString()
        {
            return "" + DeviceId + " : " + Description;
        }

    }


}
