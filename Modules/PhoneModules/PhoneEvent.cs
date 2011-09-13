using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemCore.ModuleTypes;
using System.Windows;
using PhoneModules.Controls;
using System.Windows.Controls;
using MayhemDefaultStyles.UserControls;
using System.IO;

namespace PhoneModules
{
    [DataContract]
    [MayhemModule("Phone Remote", "Triggers from phone")]
    public class PhoneEvent : EventBase, IWpfConfigurable
    {
        PhoneLayout phoneLayout = PhoneLayout.Instance;
        PhoneConnector phoneConnector = PhoneConnector.Instance;

        //---- ADDED TO AVOID ERROR
        bool IsConfiguring = false; 
        // -----------------------
      

        [DataMember]
        private string id = "";

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
                        phoneConnector.FormData = value;
                    }
                }
            }
        }

        bool isCreatingForFirstTime = false;

        public PhoneEvent()
        {
            id = Guid.NewGuid().ToString();
            isCreatingForFirstTime = true;
            phoneLayout.AddButton(id);
        }

        public override void SetConfigString()
        {
            PhoneLayoutButton button = phoneLayout.GetByID(id);
            if (button.ImageFile != null && button.ImageFile.Length > 0)
            {
                FileInfo fi = new FileInfo(button.ImageFile);
                ConfigString = fi.Name;
            }
            else
                ConfigString = button.Text;
        }

        void phoneConnector_EventCalled(string eventText)
        {
            if (eventText == id)
            {
                base.OnEventActivated();
            }
        }

        public override void Enable()
        {
            if (!IsConfiguring)
            {
                phoneLayout.EnableButton(id);

                isCreatingForFirstTime = false;
                string formData = phoneLayout.Serialize();
                phoneConnector.FormData = formData;

                phoneConnector.Enable();
                phoneConnector.EventCalled += phoneConnector_EventCalled;
            }

            base.Enable();
        }

        public override void Disable()
        {
            if (!IsConfiguring)
            {
                phoneLayout.DisableButton(id);
                phoneConnector.Disable();
                phoneConnector.EventCalled -= phoneConnector_EventCalled;
                string formData = phoneLayout.Serialize();
                phoneConnector.FormData = formData;
            }
            base.Disable();
        }

        public override void Delete()
        {
            phoneLayout.RemoveButton(id);
        }

        IWpfConfiguration IWpfConfigurable.ConfigurationControl
        {
            get
            {
                PhoneFormDesigner config = new PhoneFormDesigner(isCreatingForFirstTime);
                config.LoadFromData(id);
                //config.SetSelected(id);
                return config;
            }
        }

        public void OnSaved(IWpfConfiguration configurationControl)
        {
            if (!isCreatingForFirstTime)
            {
                string formData = phoneLayout.Serialize();
                phoneConnector.FormData = formData;
            }
            if (((PhoneFormDesigner)configurationControl).SelectedElement is PhoneUIElementButton)
            {
                PhoneUIElementButton button = ((PhoneFormDesigner)configurationControl).SelectedElement as PhoneUIElementButton;
                id = button.LayoutInfo.ID;
            }
        }
    }
}
