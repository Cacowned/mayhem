﻿using System;
using System.IO;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using PhoneModules.Controls;
using PhoneModules.Wpf;

namespace PhoneModules.Events
{
    [DataContract]
    [MayhemModule("Phone Remote", "Triggers from phone")]
    public class PhoneEvent : EventBase, IWpfConfigurable
    {
        [DataMember]
        private PhoneLayoutButton button;

        private PhoneLayout phoneLayout;
        private PhoneConnector phoneConnector;

        private bool isCreatingForFirstTime = false;

        protected override void OnBeforeLoad()
        {
            phoneLayout = PhoneLayout.Instance;
            phoneConnector = PhoneConnector.Instance;
        }

        protected override void OnLoadDefaults()
        {
            isCreatingForFirstTime = true;
            string id = Guid.NewGuid().ToString();
            button = phoneLayout.AddButton(id);
        }

        protected override void OnLoadFromSaved()
        {
            phoneLayout.AddButton(button);
        }

        public string GetConfigString()
        {
            if (!string.IsNullOrEmpty(button.ImageFile))
            {
                FileInfo fi = new FileInfo(button.ImageFile);
                return fi.Name;
            }
            else
            {
                return button.Text;
            }
        }

        private void phoneConnector_EventCalled(string eventText)
        {
            if (eventText == button.ID)
            {
                base.Trigger();
            }
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
            if (!e.WasConfiguring && !IsEnabled)
            {
                phoneLayout.EnableButton(button.ID);

                isCreatingForFirstTime = false;
                phoneConnector.SetNewData();

                phoneConnector.Enable(true);
                phoneConnector.EventCalled += phoneConnector_EventCalled;
            }
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            if (!e.IsConfiguring && IsEnabled)
            {
                phoneLayout.DisableButton(button.ID);
                phoneConnector.Disable();
                phoneConnector.EventCalled -= phoneConnector_EventCalled;
                phoneConnector.SetNewData();
            }
        }

        protected override void OnDeleted()
        {
            phoneLayout.RemoveButton(button.ID);
        }

        #region Configuration Views
        public WpfConfiguration ConfigurationControl
        {
            get
            {
                PhoneFormDesigner config = new PhoneFormDesigner(isCreatingForFirstTime);
                config.LoadFromData(button.ID);
                //config.SetSelected(id);
                return config;
            }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            if (!isCreatingForFirstTime)
            {
                phoneConnector.SetNewData();
            }
        }
        #endregion
    }
}
