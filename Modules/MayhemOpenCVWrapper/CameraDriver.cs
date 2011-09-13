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

namespace MayhemOpenCVWrapper
{


    public class CameraDriver
    {


        public const string TAG = "[MayhemCameraDriver ] : ";

        public static CameraDriver Instance = new CameraDriver();

        // capture interval determining the frame rate
    
        public bool running = false ;
        public int cWidth;
        public int cHeight;
        public CameraInfo[] devices_available = null;
        public Camera[] cameras_available = null; 
        public CameraInfo selected_device = null;

      
        public CameraDriver()
        {
            // just initialize the capture library
            // to get the camera running, InitCaptureDevice must be called!
            OpenCVDLL.OpenCVBindings.Initialize();
            devices_available = EnumerateDevices();

            // instantiate all cameras found

            cameras_available = new Camera[devices_available.Length];

            int i = 0; 

            foreach (CameraInfo c in devices_available)
            {
                Camera cam = new Camera(c, CameraSettings.DEFAULTS());
                cameras_available[i++] = cam;  
            }

            Debug.WriteLine(TAG + devices_available.Length + " devices available");

        }


       

        /**<summary>
         * Returns a string array of description of all cameras found
         * index in the array corresponds to the camera's device ID
         * </summary>
         * */
        public CameraInfo[] EnumerateDevices()
        {

            CameraInfo[] c = null;

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
           
            string[] device_strings = deviceNames.Split(';');
            //items in device_strings correspond to the actual devices

            if (device_strings.Length > 0)
            {
                c = new CameraInfo[device_strings.Length];

                for (int i = 0; i < device_strings.Length; i++)
                {
                    c[i] = new CameraInfo(i, device_strings[i]);
                }
            }

            return c;
        }

    }
}
