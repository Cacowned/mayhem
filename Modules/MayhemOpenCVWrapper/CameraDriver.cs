using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MayhemCore;

namespace MayhemOpenCVWrapper
{
    /// <summary>
    /// Manages cameras for Mayhem. Enforces the singleton pattern for camera objects available at runtime. 
    /// </summary>
    public sealed class CameraDriver
    {
        // singleton class ! 
        private static CameraDriver instance = new CameraDriver();

        /// <summary>
        /// CameraDriver singleton instance
        /// </summary>
        public static CameraDriver Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new CameraDriver();
                }

                return instance;
            }
        }

        /// <summary>
        /// Returns read-only list of the available cameras. 
        /// </summary>
        public ReadOnlyCollection<Camera> CamerasAvailable
        {
            get
            {
                return camerasAvailable.AsReadOnly();
            }
        }

        /// <summary>
        /// Returns number of available cameras.
        /// </summary>
        public int DeviceCount
        {
            get { return camerasAvailable.Count; }
        }

        private readonly List<CameraInfo> devicesAvailable;
        private readonly List<Camera> camerasAvailable;

        private CameraDriver()
        {
            devicesAvailable = new List<CameraInfo>();
            camerasAvailable = new List<Camera>();

            // just initialize the capture library
            // to get the camera running, InitCaptureDevice must be called!
            try
            {
                OpenCVDLL.OpenCVBindings.Initialize();
                devicesAvailable = EnumerateDevices();

                // instantiate all cameras found
                if (devicesAvailable.Count > 0)
                {
                    foreach (CameraInfo c in devicesAvailable)
                    {
                        Camera cam = new Camera(c, CameraSettings.Defaults());
                        camerasAvailable.Add(cam);
                    }

                    Logger.WriteLine(devicesAvailable.Count + " devices available");
                }
                else
                {
                    Logger.WriteLine("NO CAMERAS PRESENT");
                }
            }
            catch (AccessViolationException accesV)
            {
                Logger.WriteLine("AccessViolationException when Initializing Camera: \n" + accesV);
            }
        }

        /// <summary>
        /// Returns cameraInfo objects of all cameras found
        /// index in the array corresponds to the camera's device ID
        /// Works by parsing a string returned by OpenCVDLL. 
        /// TODO: Return a managed object directly from C++/Clr, getting rid of the unsafe code
        /// </summary>
        private List<CameraInfo> EnumerateDevices()
        {
            List<CameraInfo> c = new List<CameraInfo>();
            string deviceNames;

            unsafe
            {
                int deviceCount = 0;

                // deviceStrings Buffer will be written to            
                sbyte[] deviceNameBuf = new sbyte[1024];

                fixed (sbyte* deviceStrings = deviceNameBuf)
                {
                    OpenCVDLL.OpenCVBindings.EnumerateDevices(deviceStrings, &deviceCount);
                    deviceNames = new string(deviceStrings);
                }
            }

            if (deviceNames != string.Empty)
            {
                string[] deviceStrings = deviceNames.Split(';');

                // items in deviceStrings correspond to the actual devices
                if (deviceStrings.Length > 0)
                {
                    for (int i = 0; i < deviceStrings.Length; i++)
                    {
                        c.Add(new CameraInfo(i, deviceStrings[i]));
                    }
                }
            }

            return c;
        }
    }
}
