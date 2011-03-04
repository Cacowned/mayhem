using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;

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

        public override string  ToString()
        {
            return "" + deviceId + " : " + description;
        }

    }

    /// <summary>
    /// Contians settings for  cameras
    /// TODO: find out what more can be set
    /// </summary>
    public struct CameraSettings
    {
       public int resX; 
       public int resY; 
       // bpp? 
       // refresh rate ?
       // pixel format ?
       public int updateRate_ms;

       public static CameraSettings DEFAULTS()
       {
           CameraSettings cs;
           cs.resX = 320;
           cs.resY = 240;
           cs.updateRate_ms = 60;
           return cs;
       }
    
    }

    /// <summary>
    /// This class packages the functions for individual camera devices
    /// Modules using vision will now have the ability to request image updates from
    /// multiple cameras.
    /// 
    /// This class will provide the update events. 
    /// </summary>
    public class Camera
    {
        public const string TAG = "[Camera] :";


        public CameraInfo info;
        public CameraSettings settings;

        public bool is_initialized = false;
        public bool running = false;

        public int bufSize;
        public byte[] imageBuffer;

        public object thread_locker = new object();

        public delegate void ImageUpdateHandler(object sender, EventArgs e);
        public event ImageUpdateHandler OnImageUpdated;

        // (ms) : update rate with which the camera thread requests new images
        private int frameInterval;


        public Camera(CameraInfo info, CameraSettings settings)
        {
            this.info = info;
            this.settings = settings;
            InitializeCaptureDevice(info, settings);
        }

        /// <summary>
        /// Initializes the camera via the videoinput lib
        /// </summary>
        void InitializeCaptureDevice(CameraInfo info, CameraSettings settings)
        {
            is_initialized = false;

            try
            {
                OpenCVDLL.OpenCVBindings.InitCapture(info.deviceId, settings.resX, settings.resY);
                bufSize = OpenCVDLL.OpenCVBindings.GetImageSize();
                imageBuffer = new byte[bufSize];

                frameInterval = CameraSettings.DEFAULTS().updateRate_ms;

                // StartFrameGrabbing();
                is_initialized = true;
            }
            catch (Exception e)
            {
                Debug.WriteLine(TAG + "exception during camera init\n" + e.ToString());
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
                                // TODO: need to pass the camera ID here...
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
                    Debug.Write("Camera: new Frame!");
                    OnImageUpdated(this, new EventArgs());
                }

                Thread.Sleep(frameInterval);

            }
        }

        public void Release()
        {
            running = false;
            OpenCVDLL.OpenCVBindings.StopCamera(this.info.deviceId);
            is_initialized = false; 
        }


        public override string ToString()
        {
            return this.info.FriendlyName();
        }

    }

    // TODO: Add a dummy camera (in case no real device is attached!) 


    public class MayhemCameraDriver
    {


        public const string TAG = "[MayhemCameraDriver ] : ";

        public static MayhemCameraDriver Instance = new MayhemCameraDriver();

        // ms
        private const int frameInterval = 60; 

        public bool running = false ;
        public int cWidth;
        public int cHeight;

        // public int bufSize;

        public CameraInfo[] devices_available = null;

        public Camera[] cameras_available = null; 


        public CameraInfo selected_device = null;

        //public byte[] imageBuffer;

        /* doesnt have this anymore...
        public delegate void ImageUpdateHandler(object sender, EventArgs e);
        public event ImageUpdateHandler OnImageUpdated;
         */

        // public object thread_locker = new object();

        


        public MayhemCameraDriver()
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

        // old crufty code from this module (is now implemented by each camera) 
        ///**<summary>
        // * Initialiazes the capture device with the correct resolution
        // * Starts grabbing frames through the device
        // * Checks if there are devices available and that a device hasn't been selected yet.
        // * </summary>
        // */ 
        //public void InitCaptureDevice(int device, int width, int height)
        //{

        //    this.running = false;
        //    if (devices_available != null && this.selected_device == null)
        //    {
        //        cWidth = width;
        //        cHeight = height;
        //        OpenCVDLL.OpenCVBindings.InitCapture(device, width, height);
        //        bufSize = OpenCVDLL.OpenCVBindings.GetImageSize();
        //        imageBuffer = new byte[bufSize];
        //        selected_device = devices_available[device];
        //        StartFrameGrabbing();
        //    }
        //    else
        //    {
        //        Debug.WriteLine("No Cameras Available");
        //    }
        //}

        //public void StartFrameGrabbing()
        //{
        //    Thread grabFrm = new Thread(GrabFrames);
        //    try
        //    {
        //        grabFrm.Start();
        //        this.running = true;
        //    }
        //    catch (Exception e)
        //    {
        //        Debug.WriteLine("Exception while trying to start Framegrab");
        //        Debug.WriteLine(e.ToString());
        //    }
        //}

        //public void GrabFrames()
        //{
        //    running = true;

        //    while (running)
        //    {
        //        lock (thread_locker)
        //        {
        //            unsafe
        //            {

        //                fixed (byte* ptr = imageBuffer)
        //                {
        //                    try
        //                    {
        //                        OpenCVDLL.OpenCVBindings.GetNextFrame(ptr);
        //                    }
        //                    catch (Exception e)
        //                    {
        //                        Debug.WriteLine("Cam Exception " + e);
        //                        // shutdown cam
        //                        running = false;
        //                    }
        //                }
        //            }
        //        }

        //        if (OnImageUpdated != null)
        //        {
        //            Debug.Write("Camera: new Frame!");
        //            OnImageUpdated(this, new EventArgs());
        //        }

        //        Thread.Sleep(frameInterval);

        //    }
        //}

        //public void Stop()
        //{
        //    running = false;

        //}

        //public void ReleaseDevice()
        //{
        //    running = false;
        //    OpenCVDLL.OpenCVBindings.StopCamera(this.selected_device.deviceId);
        //    this.selected_device = null;
        //}
    

    }
}
