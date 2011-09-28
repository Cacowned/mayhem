﻿/*
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using System.Runtime.Serialization;
using MayhemWpf.UserControls;
using VisionModules.Wpf;
using MayhemOpenCVWrapper;
using System.Diagnostics;
using MayhemOpenCVWrapper.LowLevel;

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
    public class PresenceEvent : EventBase, IWpfConfigurable
    {

        private enum PresenceStatus
        {
            UNINITIALIZED,
            PRESENCE,
            NO_PRESENCE
        };

        // ============== presence detector and camera ==========================
        private ImagerBase cam = null;
        private CameraDriver i = CameraDriver.Instance;
        private PresenceDetectorComponent pd = null;
        private PresenceDetectorComponent.DetectionHandler presenceHandler;
        private PresenceStatus lastPresenceStatus = PresenceStatus.UNINITIALIZED;

        // ================== suppress repeat triggering
        private const int MIN_TRIGGER_INTERVAL_MS = 1500;
        private DateTime lastTriggerDate = DateTime.MinValue;

        [DataMember]
        private int selected_device_idx;

        [DataMember]
        private PresenceTriggerMode selected_trigger_mode = PresenceTriggerMode.TOGGLE;

        [DataMember]
        private double sensitivity = PresenceDetectorComponent.DEFAULT_SENSITIVITY; 

        /// <summary>
        /// Percent value of sensitivity 
        /// </summary>
        private int sensitivity_percent
        {
            get
            {
                return (int)((sensitivity - 0.005) * (100 / 0.0095));
            }
        }

        protected override void Initialize()
        {
            Logger.WriteLine("Enumerating Devices");

            if (i == null)
                i = CameraDriver.Instance;

            if (selected_device_idx < i.DeviceCount)
            {
                cam = i.cameras_available[selected_device_idx];
            }
            else
            {
                Logger.WriteLine("No camera available");
            }

            pd = new PresenceDetectorComponent(320, 240);
            presenceHandler = new PresenceDetectorComponent.DetectionHandler(m_OnPresenceUpdate);
        }

        public WpfConfiguration ConfigurationControl
        {
            get
            {
                int cam_index = 0;
                if (cam != null)
                {
                    cam_index = cam.Info.deviceId;
                }
                return new PresenceConfig(cam_index, selected_trigger_mode, sensitivity_percent);
            }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            PresenceConfig config = configurationControl as PresenceConfig;
            bool wasEnabled = this.IsEnabled;

            sensitivity = config.SelectedSensitivity;

            if (this.IsEnabled)
                this.OnDisable();

            if (cam != null)
            {
                cam = config.camera_selected;
                selected_device_idx = cam.Info.deviceId;
                selected_trigger_mode = config.selected_triggerMode;
            }
            else
            {
                cam = new DummyCamera();
            }
            if (wasEnabled)
                this.OnEnable();
        }

        protected override bool OnEnable()
        {
            Logger.WriteLine("Enable");

            if (cam != null && !IsConfiguring)
            {
                if (!cam.running)
                    cam.StartFrameGrabbing();
                pd.RegisterForImages(cam);
                pd.Sensitivity = sensitivity;
                pd.OnPresenceUpdate -= presenceHandler;
                pd.OnPresenceUpdate += presenceHandler;               
            }

            return true;
        }

        protected override void OnDisable()
        {
            Logger.WriteLine("Disable");
            // only disable the camera if the event is not configuring
            if (!IsConfiguring && cam != null)
            {
                pd.OnPresenceUpdate -= presenceHandler;
                pd.UnregisterForImages(cam);
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
                switch (selected_trigger_mode)
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
                if (activated && span.TotalMilliseconds >= MIN_TRIGGER_INTERVAL_MS)
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
            string config = "Sensitivity: " + sensitivity_percent;
            if (cam != null)
                config += ", Cam Nr: " + cam.Info.deviceId;
            return config; 
        }
    }
}
