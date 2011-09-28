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
        private string id = Guid.NewGuid().ToString();

        [DataMember]
        private string FormDataForSerialization
        {
            get
            {
                return phoneLayout.Serialize();
            }
            set
            {
                phoneLayout = PhoneLayout.Instance;
                phoneConnector = PhoneConnector.Instance;
                if (!phoneConnector.HasBeenDeserialized)
                {
                    phoneConnector.HasBeenDeserialized = true;
                    if (value != null)
                    {
                        phoneLayout.Deserialize(value);
                        phoneConnector.SetNewData();
                    }
                }
            }
        }
        #endregion

        private PhoneLayout phoneLayout = PhoneLayout.Instance;
        private PhoneConnector phoneConnector = PhoneConnector.Instance;

        private bool isCreatingForFirstTime = false;

        public PhoneEvent()
        {
            isCreatingForFirstTime = true;
            phoneLayout.AddButton(id);
        }

        public string GetConfigString()
        {
            PhoneLayoutButton button = phoneLayout.GetByID(id);
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
            if (eventText == id)
            {
                base.Trigger();
            }
        }

        protected override bool OnEnable()
        {
            if (!IsConfiguring && !IsEnabled)
            {
                phoneLayout.EnableButton(id);

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
                phoneLayout.DisableButton(id);
                phoneConnector.Disable();
                phoneConnector.EventCalled -= phoneConnector_EventCalled;
                phoneConnector.SetNewData();
            }
        }

        protected override void OnDelete()
        {
            phoneLayout.RemoveButton(id);
        }

        #region Configuration Views
        public WpfConfiguration ConfigurationControl
        {
            get
            {
                PhoneFormDesigner config = new PhoneFormDesigner(isCreatingForFirstTime);
                config.LoadFromData(id);
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
            if (((PhoneFormDesigner)configurationControl).SelectedElement is PhoneUIElementButton)
            {
                PhoneUIElementButton button = ((PhoneFormDesigner)configurationControl).SelectedElement as PhoneUIElementButton;
                id = button.LayoutInfo.ID;
            }
        }
        #endregion
    }
}
