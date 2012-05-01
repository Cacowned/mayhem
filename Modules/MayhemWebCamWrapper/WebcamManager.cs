using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.ServiceProcess;
using System.Windows.Media;
using System.Windows.Interop;
using System.Windows;
using System.Threading;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.ComponentModel;
using System.IO;
using System.Drawing.Imaging;
using Microsoft.Win32.SafeHandles;
using Microsoft.Win32;
using System.Text.RegularExpressions;
//using usbnet api from http://winusbnet.googlecode.com
using MadWizard.WinUSBNet;


namespace MayhemWebCamWrapper
{
    /// <summary>
    /// Hidden Form which we use to receive Windows messages about connected camera hardware changes...
    /// modified from http://www.codeproject.com/KB/system/DriveDetector
    /// </summary>
    ///

    internal class WebCamHardwareScannerForm : Form
    {
        private Label label1;

        public WebCamHardwareScannerForm()
        {
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Load += new System.EventHandler(this.Load_Form);
            this.Activated += new EventHandler(this.Form_Activated);
        }

        private void Load_Form(object sender, EventArgs e)
        {
            InitializeComponent();
        }

        private void Form_Activated(object sender, EventArgs e)
        {
            this.Visible = false;
            this.SendToBack();
            int pWnd = FindWindow("Progman", null);
            int tWnd = this.Handle.ToInt32();
            SetParent(tWnd, pWnd);
        }


        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(245, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "This is invisible form. To see code click View Code";
            // 
            // WebCamHardwareScannerForm
            // 
            this.ClientSize = new System.Drawing.Size(360, 80);
            this.Controls.Add(this.label1);
            this.Name = "WebCamHardwareScannerForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        [DllImport("User32.dll")]
        static extern Int32 FindWindow(String lpClassName, String lpWindowName);

        [DllImport("user32.dll")]
        static extern int SetParent(int hWndChild, int hWndNewParent);

     

    }

    /// <summary>
    /// Detects insertion or removal of webcams.
    /// </summary>
    internal class WebCamHardwareScanner : IDisposable
    {

        public WebCamHardwareScanner()
        {
            WebCamHardwareScannerForm frm = new WebCamHardwareScannerForm();
            Guid webcamGUID = new Guid("6994ad05-93ef-11d0-a3cc-00a0c9223196");
            notifier = new USBNotifier(frm, webcamGUID);
            frm.Show();
        }

        public void RegisterWebcamArrivalEvent(USBEventHandler onArrival)
        {
            if (notifier != null && onArrival != null)
                notifier.Arrival += onArrival;
        }

        public void UnregisterWebcamArrivalEvent(USBEventHandler onArrival)
        {
            if (notifier != null && onArrival != null)
                notifier.Arrival -= onArrival;
        }

        public void RegisterWebcamRemovalEvent(USBEventHandler onRemoval)
        {
            if (notifier != null && onRemoval != null)
                notifier.Removal += onRemoval;
        }

        public void UnregisterWebcamRemovalEvent(USBEventHandler onRemoval)
        {
            if (notifier != null && onRemoval != null)
                notifier.Removal -= onRemoval;
        }


        private USBNotifier notifier = null;
        public void Dispose()
        {
            if (notifier != null)
                notifier.Dispose();
        }

    }

    public class WebcamManager : IDisposable
    {
        private static WebCamHardwareScanner _scanner = null;


        public static void StartHardwareScanner()
        {
            if (!_scannerStarted && _scanner == null)
            {
                _scanner = new WebCamHardwareScanner();
                _scannerStarted = true;
            }
        }



        public static string[] GetDeviceInfoFromPath(string path)
        {
            string[] Parts = path.Split('#');
            if (Parts.Length >= 3)
            {
                Parts[0] = Parts[0].Substring(Parts[0].IndexOf(@"?\") + 2);
            }
            return Parts;
        }


        public static void StartServiceIfNeeded()
        {
            if (!_serviceStarted)
            {
                StartHardwareScanner();
                UpdateCameraList();
                _serviceStarted = true;
            }
        }

        public static void RestartService()
        {

            if (_serviceStarted)
                CleanUp();
            _serviceStarted = false;
            StartServiceIfNeeded();
        }


        public static void TerminateService()
        {
            if (_serviceStarted)
                CleanUp();
            _serviceStarted = false;
            _scanner.Dispose();
        }

        public static bool IsServiceRestartRequired()
        {
            return (DllImport.IsAnyCameraConnectedOrDisconnected() || NumberConnectedCameras() == 0);
        }

        public static void RegisterWebcamConnectionEvent(USBEventHandler onArrival)
        {
            if (_scanner != null)
                _scanner.RegisterWebcamArrivalEvent(onArrival);
        }

        public static void UnregisterWebcamConnectionEvent(USBEventHandler onArrival)
        {
            if (_scanner != null)
                _scanner.UnregisterWebcamArrivalEvent(onArrival);
        }

        public static void RegisterWebcamRemovalEvent(USBEventHandler onRemoval)
        {
            if (_scanner != null)
                _scanner.RegisterWebcamRemovalEvent(onRemoval);
        }

        public static void UnregisterWebcamRemovalEvent(USBEventHandler onRemoval)
        {
            if (_scanner != null)
                _scanner.UnregisterWebcamRemovalEvent(onRemoval);
        }

        private static void UpdateCameraList()
        {
            DllImport.RefreshWebcams();
            _numdetectedCameras = DllImport.GetNumberOfConnectedWebCams();
            _detectedCameras = new List<WebCam>();
            _availableFlags = new List<bool>();
            for (int i = 0; i < _numdetectedCameras; i++)
            {
                string cameraName = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(DllImport.GetWebCamName(i));
                string cameraPath = System.Runtime.InteropServices.Marshal.PtrToStringAnsi(DllImport.GetWebCamPath(i));
                bool isAvailable = DllImport.IsWebCamAvailable(i);
                WebCam camera = new WebCam(cameraName, cameraPath, i, isAvailable);

                _detectedCameras.Add(camera);
                _availableFlags.Add(isAvailable);
            }
        }

        public static void CleanUp()
        {
            int numberCameras = WebcamManager.NumberConnectedCameras();
            for (int i = 0; i < numberCameras; i++)
            {
                StopCamera(i);
            }
        }

        public static bool StartCamera(int index, int CaptureWidth, int CaptureHeight)
        {
            int numberCameras = WebcamManager.NumberConnectedCameras();
            if (index > -1 && index < numberCameras)
            {
                WebCam camera = WebcamManager.GetCamera(index);
                camera.Width = CaptureWidth;
                camera.Height = CaptureHeight;
                try
                {
                    camera.Start();
                }
                catch (Exception e)
                {
                    //System.Windows.Forms.MessageBox.Show(e.ToString());
                    camera.Stop();
                    return false;
                }
                return true;
            }
            return false;
        }

        public static void StopCamera(int index)
        {
            int numberCameras = WebcamManager.NumberConnectedCameras();
            if (index > -1 && index < numberCameras)
            {
                WebCam camera = WebcamManager.GetCamera(index);
                for (int i = 0; i < camera.Subscribers.Count; i++)
                {
                    ImageListenerBase l = camera.Subscribers[i];
                    l.UnregisterForImages(camera);
                }
                camera.Subscribers.Clear();
                camera.Stop();
                DllImport.StopWebCam(camera.WebCamID);
            }
        }

        public static void ReleaseInactiveCameras()
        {
            int numberCameras = WebcamManager.NumberConnectedCameras();
            for (int i = 0; i < numberCameras; i++)
            {
                WebCam camera = WebcamManager.GetCamera(i);
                if (camera.Subscribers.Count == 0)
                {
                    camera.Stop();
                    DllImport.StopWebCam(camera.WebCamID);
                }
            }
        }

        public static int NumberConnectedCameras() { return _numdetectedCameras; }

        public static WebCam GetCamera(int index)
        {
            if (index < 0 || index > _numdetectedCameras - 1)
            {
                //MessageBox.Show("Invalid camera index specified", "Webcam Acquisition Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                WebCam camera = new WebCam(default(String), default(String), -1, false);
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
        private static bool _serviceStarted;
        private static bool _scannerStarted;
    }

}
