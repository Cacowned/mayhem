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
        public int deviceId;
        public string description;

        public CameraInfo(int id, string descr)
        {
            deviceId = id;
            description = descr;
        }

        public string FriendlyName()
        {
            return this.ToString();
        }

        public override string ToString()
        {
            return "" + deviceId + " : " + description;
        }

        public static CameraInfo DummyInfo()
        {
            return new CameraInfo(0, "Dummmy Camera");
        }

    }


}
