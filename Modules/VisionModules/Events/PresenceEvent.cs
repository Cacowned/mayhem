using System;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemOpenCVWrapper;
using MayhemOpenCVWrapper.LowLevel;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using VisionModules.Wpf;

namespace VisionModules.Events
{
    /// <summary>
    /// The three modes the presence trigger can work in
    /// TOGGLE: triggers each time there is a change in presence
    /// OFF_ON: triggers when a switch from "no presence" to "presence" occurs
    /// ON_OFF: triggers when a switch from "presence" to "no presence" occurs 
    /// </summary>
    public enum PresenceTriggerMode
    {
        Toggle,
        OffOn,     // no presence  --> presence
        OnOff      // presence     --> no presence
    }

    /// <summary>
    /// Fires when it detects a change in presence.
    /// </summary>
    [DataContract]
    [MayhemModule("Presence Detector", "Detects presence of humans in the scene")]
    internal class PresenceEvent : EventBase, IWpfConfigurable
    {
        [DataMember]
        private int selectedDeviceIndex;

        [DataMember]
        private PresenceTriggerMode selectedTriggerMode;

        [DataMember]
        private double sensitivity;

        private enum PresenceStatus
        {
            Uninitialized,
            Presence,
            NoPresence
        }

        // ============== presence detector and camera ==========================
        private ImagerBase camera;
        private CameraDriver cameraDriver;
        private PresenceDetectorComponent presenceDetector;
        private PresenceDetectorComponent.DetectionHandler presenceHandler;
        private PresenceStatus lastPresenceStatus = PresenceStatus.Uninitialized;

        // ================== suppress repeat triggering
        private const int MinTriggerIntervalMs = 1500;
        private DateTime lastTriggerDate;

        public PresenceEvent()
        {
            lastTriggerDate = DateTime.MinValue;
        }

        /// <summary>
        /// Percent value of sensitivity 
        /// </summary>
        private int SensitivityPercent
        {
            get
            {
                return (int)((sensitivity - 0.005) * (100 / 0.0095));
            }
        }

        protected override void OnLoadDefaults()
        {
            selectedDeviceIndex = 0;
            selectedTriggerMode = PresenceTriggerMode.Toggle;
            sensitivity = PresenceDetectorComponent.DefaultSensitivity;
        }

        protected override void OnAfterLoad()
        {
            Logger.WriteLine("Enumerating Devices");
            cameraDriver = CameraDriver.Instance;

            if (selectedDeviceIndex < cameraDriver.DeviceCount)
            {
                camera = cameraDriver.CamerasAvailable[selectedDeviceIndex];
                presenceDetector = new PresenceDetectorComponent(320, 240);
                presenceHandler = OnPresenceUpdate;
            }
            else
            {
                Logger.WriteLine("No camera available");
            }        
        }

        public WpfConfiguration ConfigurationControl
        {
            get
            {
                int camIndex = 0;
                if (camera != null)
                {
                    camIndex = camera.Info.DeviceId;
                }
                return new PresenceConfig(camIndex, selectedTriggerMode, SensitivityPercent);
            }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            PresenceConfig config = configurationControl as PresenceConfig;

            sensitivity = config.SelectedSensitivity;
            camera = config.CameraSelected;

            if (camera != null)
            {
                selectedDeviceIndex = camera.Info.DeviceId;
                selectedTriggerMode = config.selected_triggerMode;
            }
            else
            {
                Logger.WriteLine("No camera available");
                ErrorLog.AddError(ErrorType.Warning, "PresenceDetector is disabled because no camera was detected");
                camera = null;
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected override void OnEnabling(EnablingEventArgs e)
        {
            Logger.WriteLine("Enable");

            if (!e.WasConfiguring)
            {
                if (selectedDeviceIndex < cameraDriver.DeviceCount)
                {
                    camera = cameraDriver.CamerasAvailable[selectedDeviceIndex];
                }
                else if (cameraDriver.DeviceCount > 0)
                {
                    camera = cameraDriver.CamerasAvailable[0];
                }
                else
                {
                    camera = null;
                }

                if (camera != null)
                {
                    if (!camera.Running)
                        camera.StartFrameGrabbing();
                    presenceDetector.RegisterForImages(camera);
                    presenceDetector.Sensitivity = sensitivity;
                    presenceDetector.OnPresenceUpdate -= presenceHandler;
                    presenceDetector.OnPresenceUpdate += presenceHandler;
                }
            }
            else if (camera == null)
            {
                Logger.WriteLine("No camera available");
                ErrorLog.AddError(ErrorType.Warning, "PresenceDetector is disabled because no camera was detected");
                throw new NotSupportedException("No Camera");
            }          
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        protected override void OnDisabled(DisabledEventArgs e)
        {
            // only disable the camera if the event is not configuring
            if (!e.IsConfiguring && camera != null)
            {
                presenceDetector.OnPresenceUpdate -= presenceHandler;
                presenceDetector.UnregisterForImages(camera);
                camera.TryStopFrameGrabbing();
            }
        }

        private void OnPresenceUpdate(object sender, DetectionEventArgs points)
        {
            PresenceDetectorComponent presenceComponent = sender as PresenceDetectorComponent;
            bool presence = presenceComponent.Presence;

            if (lastPresenceStatus == PresenceStatus.Uninitialized)
            {
                // do nothing!
            }
            else
            {
                bool activated = false;

                // decide whether to activate
                switch (selectedTriggerMode)
                {
                    case PresenceTriggerMode.OffOn:
                        if (lastPresenceStatus == PresenceStatus.NoPresence && presence)
                            activated = true;
                        break;
                    case PresenceTriggerMode.OnOff:
                        if (lastPresenceStatus == PresenceStatus.Presence && !presence)
                            activated = true;
                        break;
                    case PresenceTriggerMode.Toggle:
                        if (lastPresenceStatus == PresenceStatus.Presence && !presence ||
                            lastPresenceStatus == PresenceStatus.NoPresence && presence)
                            activated = true;
                        break;
                }

                TimeSpan span = DateTime.Now - lastTriggerDate;

                // activate
                if (activated && span.TotalMilliseconds >= MinTriggerIntervalMs)
                {
                    lastTriggerDate = DateTime.Now;
                    Trigger();
                }
            }

            // save the last presence state
            if (presence)
            {
                lastPresenceStatus = PresenceStatus.Presence;
            }
            else
            {
                lastPresenceStatus = PresenceStatus.NoPresence;
            }
        }

        public string GetConfigString()
        {
            string config = "Sensitivity: " + SensitivityPercent;
            if (camera != null)
                config += ", Cam Nr: " + camera.Info.DeviceId;
            return config;
        }
    }
}
