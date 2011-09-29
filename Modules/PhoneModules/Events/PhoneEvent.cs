using System;
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
        #region Configuration Properties
        [DataMember]
        private PhoneLayoutButton button;

        #endregion

        private PhoneLayout phoneLayout;
        private PhoneConnector phoneConnector;

        private bool isCreatingForFirstTime = false;

        protected override void Initialize()
        {
            phoneLayout = PhoneLayout.Instance;
            phoneConnector = PhoneConnector.Instance;
            if (button == null)
            {
                isCreatingForFirstTime = true;
                string id = Guid.NewGuid().ToString();
                button = phoneLayout.AddButton(id);
            }
            else
            {
                phoneLayout.AddButton(button);
            }
        }

        public string GetConfigString()
        {
            if (button.ImageFile != null && button.ImageFile.Length > 0)
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

        protected override bool OnEnable()
        {
            if (!IsConfiguring && !IsEnabled)
            {
                phoneLayout.EnableButton(button.ID);

                isCreatingForFirstTime = false;
                phoneConnector.SetNewData();

                phoneConnector.Enable(true);
                phoneConnector.EventCalled += phoneConnector_EventCalled;
            }

            return true;
        }

        protected override void OnDisable()
        {
            if (!IsConfiguring && IsEnabled)
            {
                phoneLayout.DisableButton(button.ID);
                phoneConnector.Disable();
                phoneConnector.EventCalled -= phoneConnector_EventCalled;
                phoneConnector.SetNewData();
            }
        }

        protected override void OnDelete()
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
