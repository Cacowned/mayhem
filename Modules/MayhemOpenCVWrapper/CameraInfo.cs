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
        public int DeviceId
        {
            get;
            private set; 
        }
        public string Description
        {
            get;
            private set; 
        }

        public CameraInfo(int id, string descr)
        {
            DeviceId = id;
            Description = descr;
        }

        public string FriendlyName()
        {
            return this.ToString();
        }

        public override string ToString()
        {
            return "" + DeviceId + " : " + Description;
        }

        public static CameraInfo DummyInfo()
        {
            return new CameraInfo(0, "Dummmy Camera");
        }

    }


}
