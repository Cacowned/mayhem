﻿using System;
using System.IO;
using System.Runtime.Serialization;
using System.Threading;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
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

        private bool isCreatingForFirstTime;

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

            return button.Text;
        }

        private void phoneConnector_EventCalled(string eventText)
        {
            if (eventText == button.Id)
            {
                Trigger();
            }
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
            if (!e.WasConfiguring && !IsEnabled)
            {
                phoneLayout.EnableButton(button.Id);

                isCreatingForFirstTime = false;

                phoneConnector.Enable(true);
                phoneConnector.SetNewData();
                phoneConnector.EventCalled += phoneConnector_EventCalled;
            }
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            if (!e.IsConfiguring && IsEnabled)
            {
                phoneLayout.DisableButton(button.Id);
                phoneConnector.Disable();
                phoneConnector.EventCalled -= phoneConnector_EventCalled;
                ThreadPool.QueueUserWorkItem(o => phoneConnector.SetNewData());
            }
        }

        protected override void OnDeleted()
        {
            phoneLayout.RemoveButton(button.Id);
        }

        #region Configuration Views
        public WpfConfiguration ConfigurationControl
        {
            get
            {
                PhoneFormDesigner config = new PhoneFormDesigner(isCreatingForFirstTime);
                config.LoadFromData(button.Id);
                return config;
            }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            if (!isCreatingForFirstTime)
            {
                ThreadPool.QueueUserWorkItem(o => phoneConnector.SetNewData());
            }
        }
        #endregion
    }
}
