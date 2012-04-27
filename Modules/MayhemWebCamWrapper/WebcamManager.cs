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



namespace MayhemWebCamWrapper
{
    /// <summary>
    /// Hidden Form which we use to receive Windows messages about connected camera hardware changes...
    /// modified from http://www.codeproject.com/KB/system/DriveDetector
    /// </summary>
    internal class WebCamHardwareScannerForm : Form
    {
        private Label label1;
        private WebCamHardwareScanner mScanner = null;
        

        public WebCamHardwareScannerForm(WebCamHardwareScanner scanner)
        {
            mScanner = scanner;
            this.MinimizeBox = false;
            this.MaximizeBox = false;
            this.ShowInTaskbar = false;
            this.ShowIcon = false;
            this.FormBorderStyle = FormBorderStyle.None;
            this.Load += new System.EventHandler(this.Load_Form);
            this.Activated += new EventHandler(this.Form_Activated);
        }

        private void Load_Form(object sender, EventArgs e)
        {
            InitializeComponent();
            this.Size = new System.Drawing.Size(5, 5);
        }

        private void Form_Activated(object sender, EventArgs e)
        {
            this.Visible = false;
        }

        protected override void WndProc(ref Message m)
        {

            /// <summary>
            /// This function receives all the windows messages for this window (form).
            /// </summary>
            base.WndProc(ref m);
            if (mScanner != null)
            {
                mScanner.WndProc(ref m);
            }
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
            this.label1.Size = new System.Drawing.Size(314, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "This is invisible form. To see code click View Code";
            // 
            // DetectorForm
            // 
            this.ClientSize = new System.Drawing.Size(360, 80);
            this.Controls.Add(this.label1);
            this.Name = "DetectorForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }
    }

    /// <summary>
    /// Our class for passing in custom arguments to our event handlers 
    /// 
    /// </summary>
    public class WebcamHardwareScannerEventArgs : EventArgs
    {


        public WebcamHardwareScannerEventArgs(string cameraName, string cameraPath)
        {
            CameraName = cameraName;
            CameraPath = cameraPath;
        }

        /// <summary>
        /// Get/Set the value indicating that the event should be cancelled 
        /// Only in QueryRemove handler.
        /// </summary>
        public string CameraName;
        public string CameraPath;
    }

    // Delegate for event handler to handle the device events 
    public delegate void WebcamHardwareScannerEventHandler(Object sender, WebcamHardwareScannerEventArgs e);

    /// <summary>
    /// Detects insertion or removal of webcams.
    /// </summary>
    class WebCamHardwareScanner : IDisposable
    {
        
        public WebCamHardwareScanner()
        {
            WebCamHardwareScannerForm frm = new WebCamHardwareScannerForm(this);
            IntPtr deviceEventHandle;
            Win32.DEV_BROADCAST_DEVICEINTERFACE devBroadCastDeviceInterface = new Win32.DEV_BROADCAST_DEVICEINTERFACE();
            IntPtr devBroadCastDeviceInterfaceBuffer = IntPtr.Zero;
            Int32 size = 0;

            size = Marshal.SizeOf(devBroadCastDeviceInterface);
            devBroadCastDeviceInterface.dbcc_size = size;
            devBroadCastDeviceInterface.dbcc_devicetype = Win32.DBT_DEVTYP_DEVICEINTERFACE;
            devBroadCastDeviceInterface.dbcc_reserved = 0;
            devBroadCastDeviceInterfaceBuffer = Marshal.AllocHGlobal(size);
            Marshal.StructureToPtr(devBroadCastDeviceInterface, devBroadCastDeviceInterfaceBuffer, true);
            deviceEventHandle = Win32.RegisterDeviceNotification(frm.Handle, devBroadCastDeviceInterfaceBuffer, Win32.DEVICE_NOTIFY_WINDOW_HANDLE | Win32.DEVICE_NOTIFY_ALL_INTERFACE_CLASSES);
            frm.Show();
        }

        public event WebcamHardwareScannerEventHandler OnWebcamConnected = null;
        public event WebcamHardwareScannerEventHandler OnWebcamDisconnected = null;

        public void Dispose()
        {
        }

        #region WindowProc
        /// <summary>
        /// Message handler which must be called from client form.
        /// Processes Windows messages and calls event handlers. 
        /// /// </summary>
        public void WndProc(ref Message m)
        {
            Win32.DEV_BROADCAST_DEVICEINTERFACE devType;
            char c;

            if (m.Msg == Win32.WM_DEVICECHANGE)
            {
                //we got a hardware change notification...
                //check if this is related to a usb webcam...
                devType = (Win32.DEV_BROADCAST_DEVICEINTERFACE)Marshal.PtrToStructure(m.LParam, typeof(Win32.DEV_BROADCAST_DEVICEINTERFACE));
                if (string.Equals(devType.dbcc_classguid.ToString(), Win32.KSCATEGORY_VIDEO.ToString()))
                {
                    string cameraPath = devType.dbcc_name;
                    string cameraName = Win32.GetDeviceName(devType);
                    switch (m.WParam.ToInt32())
                    {
                        //a new hardware device has been connected and is ready for use...

                        case Win32.DBT_DEVICEARRIVAL:
                            {
                                if (OnWebcamConnected != null)
                                    OnWebcamConnected(this, new WebcamHardwareScannerEventArgs(cameraName, cameraPath));
                                break;
                            }
                        //hardware device has been removed...
                        case Win32.DBT_DEVICEREMOVECOMPLETE:
                            {
                                if (OnWebcamDisconnected != null)
                                    OnWebcamDisconnected(this, new  WebcamHardwareScannerEventArgs(cameraName, cameraPath));
                                break;
                            }
                    }
                }
            }
        }
        #endregion

        
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

        public static void RegisterForHardwareChanges(WebcamHardwareScannerEventHandler cameraConnected, WebcamHardwareScannerEventHandler cameraDisconnected)
        {
            if (_scanner != null)
            {
                _scanner.OnWebcamConnected -= cameraConnected;
                _scanner.OnWebcamConnected += cameraConnected;
                _scanner.OnWebcamDisconnected -= cameraDisconnected;
                _scanner.OnWebcamDisconnected += cameraDisconnected;
            }
        }

        public static void UnregisterForHardwareChanges(WebcamHardwareScannerEventHandler cameraConnected, WebcamHardwareScannerEventHandler cameraDisconnected)
        {
            if (_scanner != null)
            {
                _scanner.OnWebcamConnected -= new WebcamHardwareScannerEventHandler(cameraConnected);
                _scanner.OnWebcamDisconnected -= new WebcamHardwareScannerEventHandler(cameraDisconnected);
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
                UpdateCameraList();
                _serviceStarted = true;
            }
        }

        public static void RestartService()
        {
            if (_serviceStarted)
                CleanUp();
            _serviceStarted = false;
            UpdateCameraList();
            _serviceStarted = true;
        }

    
        public static void TerminateService()
        {
            if (_serviceStarted)
                CleanUp();
            _serviceStarted = false;
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
                for (int i=0; i<camera.Subscribers.Count; i++)
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

    internal class Win32
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, IntPtr NotificationFilter, Int32 Flags);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct DEV_BROADCAST_DEVICEINTERFACE
        {
            public int dbcc_size;
            public int dbcc_devicetype;
            public int dbcc_reserved;
            public Guid dbcc_classguid;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 255)] 
            public string dbcc_name;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DEV_BROADCAST_HDR
        {
            public int dbcc_size;
            public int dbcc_devicetype;
            public int dbcc_reserved;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct DEV_BROADCAST_HANDLE
        {
            public int dbch_size;
            public int dbch_devicetype;
            public int dbch_reserved;
            public IntPtr dbch_handle;
            public IntPtr dbch_hdevnotify;
            public Guid dbch_eventguid;
            public long dbch_nameoffset;
            public byte dbch_data;
            public byte dbch_data1;
        }

        public static string GetDeviceName(DEV_BROADCAST_DEVICEINTERFACE dvi)
        {
            string[] Parts = dvi.dbcc_name.Split('#');
            if (Parts.Length >= 3)
            {
                string DevType = Parts[0].Substring(Parts[0].IndexOf(@"?\") + 2);
                string DeviceInstanceId = Parts[1];
                string DeviceUniqueID = Parts[2];
                string RegPath = @"SYSTEM\CurrentControlSet\Enum\" + DevType + "\\" + DeviceInstanceId + "\\" + DeviceUniqueID;
                RegistryKey key = Registry.LocalMachine.OpenSubKey(RegPath);
                if (key != null)
                {
                    object result = key.GetValue("FriendlyName");
                    if (result != null)
                        return result.ToString();
                    result = key.GetValue("DeviceDesc");
                    if (result != null)
                        return result.ToString();
                }
            }
            return String.Empty;
        } 


        #region Win32Const
        public static Guid KSCATEGORY_VIDEO = new Guid("6994ad05-93ef-11d0-a3cc-00a0c9223196");
        public const int DEVICE_NOTIFY_WINDOW_HANDLE = 0;
        public const int DEVICE_NOTIFY_SERVICE_HANDLE = 1;
        public const int DEVICE_NOTIFY_ALL_INTERFACE_CLASSES = 4;
        public const int DBT_DEVTYP_DEVICEINTERFACE = 5;
        public const int DBT_DEVTYP_HANDLE = 6;
        public const int WM_DEVICECHANGE = 0x0219;
        public const int DBT_DEVICEARRIVAL = 0x8000; // system detected a new device
        public const int DBT_DEVICEQUERYREMOVE = 0x8001;   // Preparing to remove (any program can disable the removal)
        public const int DBT_DEVICEREMOVECOMPLETE = 0x8004; // removed 
        #endregion
    }
}
