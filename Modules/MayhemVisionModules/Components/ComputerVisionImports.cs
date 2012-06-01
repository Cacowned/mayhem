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

        //1) motion detection
        [DllImport("WebCamLib.dll")]
        public extern static IntPtr CreateMotionDetector();

        [DllImport("WebCamLib.dll")]
        public extern static void DisposeMotionDetector(IntPtr pObject);

        [DllImport("WebCamLib.dll")]
        public extern static bool UpdateBackground(IntPtr pObject, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I1, SizeParamIndex = 0)] byte[] inImage, [MarshalAs(UnmanagedType.LPArray, ArraySubType = UnmanagedType.I1, SizeParamIndex = 0)] byte[] outImage, int width, int height, float detectorPercentage, float detectorDifference, float detectorTime, int roix, int roiy, int roiWidth, int roiHeight);

        [DllImport("WebCamLib.dll")]
        public extern static bool IsMotionDetected(IntPtr pObject);

    }
}
