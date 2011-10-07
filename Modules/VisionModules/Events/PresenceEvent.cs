/*
 * 
 * PresenceEvent.cs
 * 
 * Presence Mayhem Event
 * 
 * Fires when it detects a change in presence. 
 * 
 * (c) 2011, Microsoft Applied Sciences Group
 * 
 * Author: Sven Kratz
 * 
 */

using System;
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
        TOGGLE,
        OFF_ON,     // no presence  --> presence
        ON_OFF      // presence     --> no presence
    }

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
            UNINITIALIZED,
            PRESENCE,
            NO_PRESENCE
        };

        // ============== presence detector and camera ==========================
        private ImagerBase cam = null;
        private CameraDriver cameraDriver;
        private PresenceDetectorComponent presenceDetector = null;
        private PresenceDetectorComponent.DetectionHandler presenceHandler;
        private PresenceStatus lastPresenceStatus = PresenceStatus.UNINITIALIZED;

        // ================== suppress repeat triggering
        private const int MinTriggerIntervalMS = 1500;
        private DateTime lastTriggerDate = DateTime.MinValue;

        /// <summary>
        /// Percent value of sensitivity 
        /// </summary>
        private int sensitivityPercent
        {
            get
            {
                return (int)((sensitivity - 0.005) * (100 / 0.0095));
            }
        }

        protected override void OnLoadDefaults()
        {
            selectedDeviceIndex = 0;
            selectedTriggerMode = PresenceTriggerMode.TOGGLE;
            sensitivity = PresenceDetectorComponent.DEFAULT_SENSITIVITY;
        }

        protected override void OnAfterLoad()
        {
            Logger.WriteLine("Enumerating Devices");

            cameraDriver = CameraDriver.Instance;

            if (selectedDeviceIndex < cameraDriver.DeviceCount)
            {
                cam = cameraDriver.CamerasAvailable[selectedDeviceIndex];
            }
            else
            {
                Logger.WriteLine("No camera available");
            }

            presenceDetector = new PresenceDetectorComponent(320, 240);
            presenceHandler = new PresenceDetectorComponent.DetectionHandler(m_OnPresenceUpdate);
        }

        public WpfConfiguration ConfigurationControl
        {
            get
            {
                int cam_index = 0;
                if (cam != null)
                {
                    cam_index = cam.Info.DeviceId;
                }
                return new PresenceConfig(cam_index, selectedTriggerMode, sensitivityPercent);
            }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            PresenceConfig config = configurationControl as PresenceConfig;
            bool wasEnabled = this.IsEnabled;

            sensitivity = config.SelectedSensitivity;

            if (cam != null)
            {
                cam = config.camera_selected;
                selectedDeviceIndex = cam.Info.DeviceId;
                selectedTriggerMode = config.selected_triggerMode;
            }
            else
            {
                cam = new DummyCamera();
            }
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
            Logger.WriteLine("Enable");

            if (cam != null && !e.WasConfiguring)
            {
                if (!cam.Running)
                    cam.StartFrameGrabbing();
                presenceDetector.RegisterForImages(cam);
                presenceDetector.Sensitivity = sensitivity;
                presenceDetector.OnPresenceUpdate -= presenceHandler;
                presenceDetector.OnPresenceUpdate += presenceHandler;
            }
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            Logger.WriteLine("Disable");
            // only disable the camera if the event is not configuring
            if (!e.IsConfiguring && cam != null)
            {
                presenceDetector.OnPresenceUpdate -= presenceHandler;
                presenceDetector.UnregisterForImages(cam);
                cam.TryStopFrameGrabbing();
            }
        }

        public void m_OnPresenceUpdate(object sender, System.Drawing.Point[] points)
        {
            PresenceDetectorComponent presenceDetector = sender as PresenceDetectorComponent;
            bool presence = presenceDetector.presence;

            if (lastPresenceStatus == PresenceStatus.UNINITIALIZED)
            {
                // do nothing!
            }
            else
            {
                bool activated = false;

                // decide whether to activate
                switch (selectedTriggerMode)
                {

                    case PresenceTriggerMode.OFF_ON:
                        if (lastPresenceStatus == PresenceStatus.NO_PRESENCE && presence == true)
                            activated = true;
                        break;
                    case PresenceTriggerMode.ON_OFF:
                        if (lastPresenceStatus == PresenceStatus.PRESENCE && presence == false)
                            activated = true;
                        break;
                    case PresenceTriggerMode.TOGGLE:
                        if (lastPresenceStatus == PresenceStatus.PRESENCE && presence == false ||
                            lastPresenceStatus == PresenceStatus.NO_PRESENCE && presence == true)
                            activated = true;
                        break;
                }

                TimeSpan span = DateTime.Now - lastTriggerDate;

                // activate
                if (activated && span.TotalMilliseconds >= MinTriggerIntervalMS)
                {
                    lastTriggerDate = DateTime.Now;
                    base.Trigger();
                }
            }

            // save the last presence state
            if (presence)
            {
                lastPresenceStatus = PresenceStatus.PRESENCE;
            }
            else
            {
                lastPresenceStatus = PresenceStatus.NO_PRESENCE;
            }
        }

        public string GetConfigString()
        {
            string config = "Sensitivity: " + sensitivityPercent;
            if (cam != null)
                config += ", Cam Nr: " + cam.Info.DeviceId;
            return config;
        }
    }
}
