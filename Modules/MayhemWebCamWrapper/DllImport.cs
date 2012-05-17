using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace MayhemWebCamWrapper
{
    public class DllImport
    {
        [DllImport("WebCamLib.dll")]
        public extern static void InitializeWebcams();

        [DllImport("WebCamLib.dll")]
        public extern static void RefreshWebcams();

        [DllImport("WebCamLib.dll")]
        public extern static int GetNumberOfConnectedWebCams();

        [DllImport("WebCamLib.dll")]
        public extern static bool IsAnyCameraConnectedOrDisconnected();

        [DllImport("WebCamLib.dll")]
        public extern static IntPtr GetWebCamName(int index);

        [DllImport("WebCamLib.dll")]
        public extern static IntPtr GetWebCamPath(int index);

        [DllImport("WebCamLib.dll")]
        public extern static bool IsWebCamAvailable(int index);

        [DllImport("WebCamLib.dll")]
        public extern static bool StartWebCam(int index);

        [DllImport("WebCamLib.dll")]
        public extern static bool StopWebCam(int index);

        [DllImport("WebCamLib.dll")]
        public extern static bool GrabFrameWebcam(int cameraindex, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I1, SizeParamIndex = 0)] byte[] outImage, int width, int height);

        [DllImport("WebCamLib.dll")]
        public extern static float GetFrameRate(int cameraindex);

        [DllImport("WebCamLib.dll")]
        public extern static bool ConfigureCameraProperties(int cameraindex);

        
        //programatically setting the camera properties...
        //prop = 0 for focus, prop = 1 for exposure, prop = 2 for zoom
        [DllImport("WebCamLib.dll")]
        public extern static bool GetCurrentPropertyValue(int cameraindex, int prop, ref long val);

        [DllImport("WebCamLib.dll")]
	    public extern static bool GetDefaultPropertyValue(int cameraindex, int prop, ref long val);

        [DllImport("WebCamLib.dll")]
	    public extern static bool GetMinPropertyValue(int cameraindex, int prop, ref long val);

        [DllImport("WebCamLib.dll")]
	    public extern static bool GetMaxPropertyValue(int cameraindex, int prop, ref long val);

        [DllImport("WebCamLib.dll")]
        public extern static bool SetPropertyValue(int cameraindex, int prop, ref long val);
	    
        [DllImport("WebCamLib.dll")]
	    public extern static bool SetPropertyValueAuto(int cameraindex, int prop);

    }
}
