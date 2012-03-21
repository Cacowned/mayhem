using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows;
using System.Threading;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.IO;
using System.Drawing.Imaging;



namespace MayhemWebCamWrapper
{
    public class WebcamManager : System.Windows.Controls.TextBlock, IDisposable
    {
        public WebcamManager()
        {
            DllImport.RefreshWebcams();
            System.Windows.Application.Current.Exit += WebCamManagerCleanup;
        }
       
        private void WebCamManagerCleanup(object sender, EventArgs e)
        {
            CleanUp();
        }
        public static void UpdateCameraList()
        {
            DllImport.RefreshWebcams();
            _numdetectedCameras = DllImport.GetNumberOfConnectedWebCams();
            _detectedCameras = new List<WebCam>();
            _availableFlags = new List<bool>();
            for (int i = 0; i < _numdetectedCameras; i++)
            {
                string cameraName = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(DllImport.GetWebCamName(i));
                bool isAvailable = DllImport.IsWebCamAvailable(i);
                WebCam camera = new WebCam(cameraName, i, isAvailable);

                _detectedCameras.Add(camera);
                _availableFlags.Add(isAvailable);
            }
        }

        public static void CleanUp()
        {
            int numberCameras = WebcamManager.NumberConnectedCameras();
            for (int i = 0; i < numberCameras; i++)
            {
                WebCam camera = WebcamManager.GetCamera(i);
                camera.Stop();
            }
            for (int i = 0; i < numberCameras; i++)
            {
                WebCam camera = WebcamManager.GetCamera(i);
                DllImport.StopWebCam(camera.WebCamID);
            }
        }

        public static int NumberConnectedCameras() { return _numdetectedCameras; }
        
        public static WebCam GetCamera(int index)
        {
            if (index < 0 || index > _numdetectedCameras - 1)
            {
                //MessageBox.Show("Invalid camera index specified", "Webcam Acquisition Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                WebCam camera = new WebCam("", -1, false);
                return camera;
            }
            else
            {
                return _detectedCameras[index];
            }
        }

        public void Dispose()
        {
            CleanUp();
        }

        private static List<WebCam> _detectedCameras;
        private static List<bool> _availableFlags;
        private static int _numdetectedCameras;
    }

}
