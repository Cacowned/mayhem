/*
 *  MayhemCameraDriver.cs
 *  
 *  Manages cameras for Mayhem. Enforces the singleton pattern for camera objects available at runtime. 
 * 
 * 
 *  (c) 2010/2011, Microsoft Applied Sciences Group
 *  
 *  Author: Sven Kratz
 * 
 */

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MayhemCore;

namespace MayhemOpenCVWrapper
{
    public sealed class CameraDriver
    {
        // singleton class ! 
        private static CameraDriver instance_ = new CameraDriver();

        /// <summary>
        /// CameraDriver singleton instance
        /// </summary>
        public static CameraDriver Instance
        {
            get
            {
                if (instance_ == null)
                {
                    instance_ = new CameraDriver();
                }
                return instance_;
            }
        }

        /// <summary>
        /// Returns readonly list of the available cameras. 
        /// </summary>
        public  ReadOnlyCollection<Camera> CamerasAvailable
        {
            get { return cameras_available_.AsReadOnly(); }
        }

        /// <summary>
        /// Returns number of available cameras.
        /// </summary>
        public int DeviceCount
        {
            get { return cameras_available_.Count; }
        }

        private List<CameraInfo> devices_available = new List<CameraInfo>();
        private List<Camera> cameras_available_ = new List<Camera>(); 
      
        private CameraDriver()
        {
            // just initialize the capture library
            // to get the camera running, InitCaptureDevice must be called!
            try
            {
                OpenCVDLL.OpenCVBindings.Initialize();
                devices_available = EnumerateDevices();

                // instantiate all cameras found
                if (devices_available.Count > 0)
                {

                    foreach (CameraInfo c in devices_available)
                    {
                        Camera cam = new Camera(c, CameraSettings.Defaults());
                        cameras_available_.Add(cam);
                    }

                    Logger.WriteLine(devices_available.Count + " devices available");
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
        ///  Returns cameraInfo objects of all cameras found
        ///  index in the array corresponds to the camera's device ID
        ///  Works by parsing a string returned by OpenCVDLL. 
        ///  TODO: Return a managed object directly from C++/Clr, getting rid of the unsafe code
        /// </summary>
        private List<CameraInfo> EnumerateDevices()
        {
            List<CameraInfo> c = new List<CameraInfo>();
            string deviceNames = "";

            unsafe
            {
                int deviceCount = 0;
                // deviceStrings Buffer will be written to            
                sbyte[] deviceNameBuf =  new sbyte[1024];
                
                fixed (sbyte* deviceStrings = deviceNameBuf)
                {
                    OpenCVDLL.OpenCVBindings.EnumerateDevices(deviceStrings, &deviceCount);
                    deviceNames = new String(deviceStrings);                 
                }       
            }
            if (deviceNames != string.Empty)
            {
                string[] deviceStrings = deviceNames.Split(';');
                //items in device_strings correspond to the actual devices

                if (deviceStrings.Length > 0)
                {                  
                    for (int i = 0; i < deviceStrings.Length; i++)
                    {
                        c.Add( new CameraInfo(i, deviceStrings[i]) ) ;
                    }
                }
            }
            return c;
        }
    }
}
