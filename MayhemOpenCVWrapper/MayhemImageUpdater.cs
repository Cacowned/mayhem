using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;

namespace MayhemOpenCVWrapper
{

    /* just a wrapper for information about detected cameras 
       could in future be extended to contain the camera's capabilities (i.e. max resolutions, etc.)
     */
    public class CameraInfo
    {
        public int deviceId;
        public string description;

        public CameraInfo(int id, string descr)
        {
            deviceId = id;
            description = descr;
        }
    }

    public class MayhemImageUpdater
    {

        public static MayhemImageUpdater Instance = new MayhemImageUpdater();

        // ms
        private const int frameInterval = 60; 

        public bool running = false ;
        public int cWidth;
        public int cHeight;

        public int bufSize;

        public CameraInfo[] devices_available = null;

        public CameraInfo selected_device = null;

        public byte[] imageBuffer;

        public delegate void ImageUpdateHandler(object sender, EventArgs e);
        public event ImageUpdateHandler OnImageUpdated;

        public object thread_locker = new object();

        


        public MayhemImageUpdater()
        {
            // just initialize the capture library
            // to get the camera running, InitCaptureDevice must be called!
            OpenCVDLL.OpenCVBindings.Initialize();
            devices_available = EnumerateDevices();
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
                    //sbyte** deviceStrings = (sbyte**) p;
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

        /**<summary>
         * Initialiazes the capture device with the correct resolution
         * Starts grabbing frames through the device
         * Checks if there are devices available and that a device hasn't been selected yet.
         * </summary>
         */ 
        public void InitCaptureDevice(int device, int width, int height)
        {

            this.running = false;
            if (devices_available != null && this.selected_device == null)
            {
                cWidth = width;
                cHeight = height;
                OpenCVDLL.OpenCVBindings.InitCapture(device, width, height);
                bufSize = OpenCVDLL.OpenCVBindings.GetImageSize();
                imageBuffer = new byte[bufSize];
                selected_device = devices_available[device];
                StartFrameGrabbing();
            }
            else
            {
                Debug.WriteLine("No Cameras Available");
            }
        }

        public void StartFrameGrabbing()
        {
            Thread grabFrm = new Thread(GrabFrames);
            try
            {
                grabFrm.Start();
                this.running = true;
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception while trying to start Framegrab");
                Debug.WriteLine(e.ToString());
            }
        }

        public void GrabFrames()
        {
            running = true;

            while (running)
            {
                lock (thread_locker)
                {
                    unsafe
                    {

                        fixed (byte* ptr = imageBuffer)
                        {
                            try
                            {
                                OpenCVDLL.OpenCVBindings.GetNextFrame(ptr);
                            }
                            catch (Exception e)
                            {
                                Debug.WriteLine("Cam Exception " + e);
                                // shutdown cam
                                running = false;
                            }
                        }
                    }
                }

                if (OnImageUpdated != null)
                {
                    OnImageUpdated(this, new EventArgs());
                }

                Thread.Sleep(frameInterval);

            }
        }

        public void Stop()
        {
            running = false;

        }

        public void ReleaseDevice()
        {
            running = false;
            OpenCVDLL.OpenCVBindings.StopCamera(this.selected_device.deviceId);
            this.selected_device = null;
        }
    

    }
}
