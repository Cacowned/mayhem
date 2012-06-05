using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemWebCamWrapper;
using MayhemVisionModules.Wpf;
using MayhemWpf.UserControls;
using MayhemWpf.ModuleTypes;
using MayhemVisionModules.Components;
using System.Windows.Forms;
using System.Media;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Media.Imaging;
using System.Runtime.CompilerServices;
using MadWizard.WinUSBNet;
using System.Windows.Threading;
using System.Threading;
using System.IO;



namespace MayhemVisionModules.Events
{
    [DataContract]
    [MayhemModule("Motion Detection", "Triggers when a webcam detects motion")] 
    public class WebcamMotionDetection : EventBase, IWpfConfigurable
    {
        [DataMember]
        private string selectedCameraPath; //this is the selected camera

        [DataMember]
        private string selectedCameraName;

        [DataMember]
        private int selectedCameraIndex;

        [DataMember]
        private bool selectedCameraConnected;

        [DataMember]
        private int captureWidth;

        [DataMember]
        private int captureHeight;

        [DataMember]
        private float percentageSensitivity;

        [DataMember]
        private int differenceThreshold;

        [DataMember]
        private float timeThreshold;

        [DataMember]
        private double roiX;

        [DataMember]
        private double roiY;

        [DataMember]
        private double roiWidth;

        [DataMember]
        private double roiHeight;

        [DataMember]
        private int camerafocus;

        [DataMember]
        private int camerazoom;

        private WebCamMotionDetector motionDetector; //this registers to provide motion detector triggers

        bool callbacksRegistered = false;

        ~WebcamMotionDetection()
        {
            if (selectedCameraIndex != -1 && selectedCameraConnected && motionDetector != null)
            {
                ReleasePreviousDetectors();
                motionDetector.UnregisterForImages(WebcamManager.GetCamera(selectedCameraIndex));
            }
            WebcamManager.ReleaseInactiveCameras();
            if (callbacksRegistered)
            {
                WebcamManager.UnregisterWebcamConnectionEvent(OnCameraConnected);
                WebcamManager.UnregisterWebcamRemovalEvent(OnCameraDisconnected);
                callbacksRegistered = false;
            }
        }

        private void WebCamMotionDetected(object o, EventArgs a)
        {
            Trigger();
        }

        protected int LookforSelectedCamera(bool force = false)
        {
            bool selectedCameraFound = false;
            selectedCameraConnected = false;
            if (!IsEnabled && !force)
                return -1;
            int numberConnectedCameras = WebcamManager.NumberConnectedCameras();
            if (numberConnectedCameras == 0)
            {
                selectedCameraConnected = false;
                Logger.WriteLine("No camera available");
                ErrorLog.AddError(ErrorType.Failure, "Motion detection is disabled because no camera was detected");
                return -1;
            }
            int index = -1;
            if (selectedCameraPath == default(string))
            {
                ErrorLog.AddError(ErrorType.Message, "No webcam configuration. Defaulting to first available webcam.");
                return -1;
            }
            else
            {
                for (int i = 0; i < numberConnectedCameras; i++)
                {
                    if (WebcamManager.GetCamera(i).WebCamPath == selectedCameraPath)
                    {
                        if (WebcamManager.GetCamera(i).IsActive)
                        {
                            index = i;
                            selectedCameraConnected = true;
                            selectedCameraFound = true;
                            break;

                        }
                        else if (WebcamManager.StartCamera(i, captureWidth, captureHeight))
                        {
                            index = i;
                            selectedCameraConnected = true;
                            selectedCameraFound = true;
                            break;
                        }
                    }
                }

                if (!selectedCameraFound && numberConnectedCameras > 0)
                {
                    ErrorLog.AddError(ErrorType.Failure, "The originally selected camera is not available.");
                }
                return index;
            }
        }

        protected void ReleasePreviousDetectors()
        {
            
            if (motionDetector != null && selectedCameraIndex != -1)
            {
                for (int i = 0; i < motionDetector.SubscribedImagers.Count; i++)
                    motionDetector.UnregisterForImages(motionDetector.SubscribedImagers[i]);
                motionDetector.Clear();
                motionDetector = null;
            }
        }

        public void OnCameraConnected(object sender, USBEvent e)
        {
            if (!selectedCameraConnected)
            {
                Thread.Sleep(50);
                WebcamManager.RestartService();
                selectedCameraIndex = LookforSelectedCamera();
                if (selectedCameraIndex != -1)
                {
                    Logger.WriteLine("Selected camera reconnected");
                    ErrorLog.AddError(ErrorType.Message, "Selected camera has been reconnected");
                    motionDetector.RegisterForImages(WebcamManager.GetCamera(selectedCameraIndex));
                    WebcamManager.ReleaseInactiveCameras();
                }

            }
        }

        public void OnCameraDisconnected(object sender, USBEvent e)
        {
            string[] changedParts = WebcamManager.GetDeviceInfoFromPath(e.DevicePath);
            string[] selectedParts = WebcamManager.GetDeviceInfoFromPath(selectedCameraPath);
            if (string.Compare(changedParts[1], selectedParts[1], true) == 0 && string.Compare(changedParts[2], selectedParts[2], true) == 0)
            {
                selectedCameraConnected = false;
                ReleasePreviousDetectors();
                WebcamManager.ReleaseInactiveCameras();
                Logger.WriteLine("Selected camera disconnected");
                ErrorLog.AddError(ErrorType.Warning, "Selected camera has been disconnected");
            }
        }

        
        protected override void OnLoadDefaults()
        {
            captureWidth = 640;
            captureHeight = 480;
            percentageSensitivity = 5;
            differenceThreshold = 30;
            timeThreshold = 1;
            roiX = 0;
            roiY = 0;
            roiWidth = 1;
            roiHeight = 1;
            camerafocus = 0;
            camerazoom = 1;
        }

        protected override void OnLoadFromSaved()
        {
            captureWidth = 640;
            captureHeight = 480;
        }


        protected override void OnAfterLoad()
        {

            selectedCameraIndex = LookforSelectedCamera();
            if (selectedCameraIndex != -1 && selectedCameraConnected)
            {
                if (WebcamManager.IsServiceRestartRequired())
                    WebcamManager.RestartService();
                if (!callbacksRegistered)
                {
                    WebcamManager.RegisterWebcamConnectionEvent(OnCameraConnected);
                    WebcamManager.RegisterWebcamRemovalEvent(OnCameraDisconnected);
                    callbacksRegistered = true;
                }
                InitializeMotionDetection(selectedCameraIndex);
            }
 
        }

        //[MethodImpl(MethodImplOptions.Synchronized)]
        protected override void OnDisabled(DisabledEventArgs e)
        {
            if (!e.IsConfiguring)
            {
                if (motionDetector != null && selectedCameraIndex != -1)
                {
                    motionDetector.MotionDetected -= WebCamMotionDetected;
                    motionDetector.UnregisterForImages(WebcamManager.GetCamera(selectedCameraIndex));
                    ReleasePreviousDetectors();
                    
                    WebcamManager.SetPropertyValueAuto(selectedCameraIndex, WebcamManager.CAMERA_PROPERTY.CAMERA_FOCUS);
                    WebcamManager.SetPropertyValueAuto(selectedCameraIndex, WebcamManager.CAMERA_PROPERTY.CAMERA_ZOOM);
                    WebcamManager.ReleaseInactiveCameras();
                    if (callbacksRegistered)
                    {
                        WebcamManager.UnregisterWebcamConnectionEvent(OnCameraConnected);
                        WebcamManager.UnregisterWebcamRemovalEvent(OnCameraDisconnected);
                        callbacksRegistered = false;
                    }
                }
            }
        }


        void InitializeMotionDetection(int cameraindex)
        {
            
            Thread thread = new Thread(() =>
            {
                try
                {
                    ReleasePreviousDetectors();
                    WebcamManager.SetPropertyValueManual(cameraindex, WebcamManager.CAMERA_PROPERTY.CAMERA_FOCUS, camerafocus);
                    WebcamManager.SetPropertyValueManual(cameraindex, WebcamManager.CAMERA_PROPERTY.CAMERA_ZOOM, camerazoom);
                    
                    motionDetector = new WebCamMotionDetector();
                    motionDetector.ToggleVisualization();
                    motionDetector.RegisterForImages(WebcamManager.GetCamera(cameraindex));
                    motionDetector.SelectedCameraIndex = cameraindex;
                    motionDetector.MotionAreaPercentageSensitivity = percentageSensitivity;
                    motionDetector.TimeSensitivity = timeThreshold;
                    motionDetector.RoiX = roiX;
                    motionDetector.RoiY = roiY;
                    motionDetector.RoiWidth = roiWidth;
                    motionDetector.RoiHeight = roiHeight;
                    motionDetector.MotionDiffSensitivity = differenceThreshold;
                    motionDetector.MotionDetected += WebCamMotionDetected;
                }
                catch (Exception err)
                {
                    MessageBox.Show(err.ToString());
                }
           
            });
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();
        }

        //[MethodImpl(MethodImplOptions.Synchronized)]
        protected override void OnEnabling(EnablingEventArgs e)
        {
            if (!e.WasConfiguring)
            {
                if (WebcamManager.IsServiceRestartRequired())
                    WebcamManager.RestartService();
                selectedCameraIndex = LookforSelectedCamera(true);
                
                if (selectedCameraIndex != -1 && selectedCameraConnected)
                {
                    if (!callbacksRegistered)
                    {
                        WebcamManager.RegisterWebcamConnectionEvent(OnCameraConnected);
                        WebcamManager.RegisterWebcamRemovalEvent(OnCameraDisconnected);
                        callbacksRegistered = true;
                    }
                    InitializeMotionDetection(selectedCameraIndex);
                }
            }
        }


        public void OnSaved(WpfConfiguration configurationControl)
        {
            WebCamMotionDetectionConfig config = configurationControl as WebCamMotionDetectionConfig;
            config.Cleanup();
            captureWidth = 640;
            captureHeight = 480;
            selectedCameraPath = config.SelectedCameraPath;
            selectedCameraName = config.SelectedCameraName;
            selectedCameraIndex = config.SelectedCameraIndex;
            percentageSensitivity = config.PercentageThresh;
            differenceThreshold = config.DiffThresh;
            timeThreshold = config.TimeThresh;
            roiX = config.RoiX;
            roiY = config.RoiY;
            roiWidth = config.RoiWidth;
            roiHeight = config.RoiHeight;
            camerafocus = config.CameraFocus;
            camerazoom = config.CameraZoom;

            if (WebcamManager.IsServiceRestartRequired())
                WebcamManager.RestartService();
            selectedCameraIndex = LookforSelectedCamera(true);

            if (selectedCameraIndex != -1 && selectedCameraConnected)
            {
                if (!callbacksRegistered)
                {
                    WebcamManager.RegisterWebcamConnectionEvent(OnCameraConnected);
                    WebcamManager.RegisterWebcamRemovalEvent(OnCameraDisconnected);
                    callbacksRegistered = true;
                }
                InitializeMotionDetection(selectedCameraIndex);
            }

        }

        protected override void OnDeleted()
        {
            if (motionDetector != null && selectedCameraIndex != -1)
            {
                motionDetector.MotionDetected -= WebCamMotionDetected;
                ReleasePreviousDetectors();
                WebcamManager.SetPropertyValueAuto(selectedCameraIndex, WebcamManager.CAMERA_PROPERTY.CAMERA_FOCUS);
                WebcamManager.SetPropertyValueAuto(selectedCameraIndex, WebcamManager.CAMERA_PROPERTY.CAMERA_ZOOM);
                WebcamManager.ReleaseInactiveCameras();
                if (callbacksRegistered)
                {
                    WebcamManager.UnregisterWebcamConnectionEvent(OnCameraConnected);
                    WebcamManager.UnregisterWebcamRemovalEvent(OnCameraDisconnected);
                    callbacksRegistered = false;
                }
            }
        }

        
       

        public WpfConfiguration ConfigurationControl
        {
            get
            {
                WebCamMotionDetectionConfig config = new WebCamMotionDetectionConfig(camerafocus, camerazoom, percentageSensitivity, timeThreshold, differenceThreshold, roiX, roiY, roiWidth, roiHeight);
                config.Cleanup();
                return config;
            }
        }

        public string GetConfigString()
        {
            return string.Format("\"{0}\" ", selectedCameraName);
        }
    }
}
