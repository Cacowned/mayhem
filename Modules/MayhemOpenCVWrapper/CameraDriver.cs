/*
 *  MayhemCameraDriver.cs
 *  
 *  Manages cameras for Mayhem
 * 
 *  (c) 2010/2011, Microsoft Applied Sciences Group
 *  
 *  Author: Sven Kratz
 * 
 */ 

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using MayhemCore;

namespace MayhemOpenCVWrapper
{
    public class CameraDriver
    {
        public static CameraDriver Instance = new CameraDriver();
    
        public bool running = false ;
        public int cWidth;
        public int cHeight;
        private List<CameraInfo> devices_available = new List<CameraInfo>();
        public List<Camera> cameras_available = new List<Camera>(); 
        public CameraInfo selected_device = null;

        public int DeviceCount
        {
            get { return cameras_available.Count; }
        }

      
        public CameraDriver()
        {
            // just initialize the capture library
            // to get the camera running, InitCaptureDevice must be called!
            OpenCVDLL.OpenCVBindings.Initialize();
            devices_available = EnumerateDevices();

            // instantiate all cameras found
            if (devices_available.Count > 0)
            {

                foreach (CameraInfo c in devices_available)
                {
                    Camera cam = new Camera(c, CameraSettings.DEFAULTS());
                    cameras_available.Add(cam);
                }

                Logger.WriteLine(devices_available.Count + " devices available");
            }
            else
            {
                Logger.WriteLine("NO CAMERAS PRESENT");
            }
        }

        /**<summary>
         * Returns a string array of description of all cameras found
         * index in the array corresponds to the camera's device ID
         * </summary>
         * */
        public List<CameraInfo> EnumerateDevices()
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
                string[] device_strings = deviceNames.Split(';');
                //items in device_strings correspond to the actual devices

                if (device_strings.Length > 0)
                {                  
                    for (int i = 0; i < device_strings.Length; i++)
                    {
                        c.Add( new CameraInfo(i, device_strings[i]) ) ;
                    }
                }
            }
            return c;
        }

    }
}
