using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace MayhemVisionModules.Components
{
    public class ComputerVisionImports
    {
        //computer vision stuff
        //event handler for various computer vision events...
        public delegate void ComputerVisionEventHandler(object sender, EventArgs e);


        //1) motion detector
        [DllImport("WebCamLib.dll")]
        public extern static bool RunMotionDetectWebcam(int cameraindex, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I1, SizeParamIndex = 0)] byte[] outImage, int width, int height);

        [DllImport("WebCamLib.dll")]
        public extern static bool IsMotionDetected(int cameraindex);

        [DllImport("WebCamLib.dll")]
        public extern static void MotionDetectorPercentage(int cameraindex, float value);

        [DllImport("WebCamLib.dll")]
        public extern static void MotionDetectorDifference(int cameraindex, float value);

        [DllImport("WebCamLib.dll")]
        public extern static void MotionDetectorTime(int cameraindex, float value);

        [DllImport("WebCamLib.dll")]
        public extern static void MotionDetectorROI(int cameraindex, int roix, int roiy, int roiWidth, int roiHeight);

        [DllImport("WebCamLib.dll")]
        public extern static void MotionDetectorClear(int cameraindex);

    }
}
