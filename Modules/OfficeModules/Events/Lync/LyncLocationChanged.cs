using System;
using System.Globalization;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Group;
using OfficeModules.Resources;
using OfficeModules.Wpf;

namespace OfficeModules.Events.Lync
{
    /// <summary>
    /// This event is triggered when the location of a predefined contact changes
    /// </summary>
    [DataContract]
    [MayhemModule("Lync: Location Changed", "Triggers when the location changes")]
    public class LyncLocationChanged : EventBase, IWpfConfigurable
    {
        [DataMember]
        private string userId;

        private LyncClient lyncClient;
        private Contact selectedContact;
        private string currentLocation;

        private EventHandler<ContactInformationChangedEventArgs> contactInformationChanged;

        protected override void OnAfterLoad()
        {
            lyncClient = null;
            selectedContact = null;

            contactInformationChanged = Contact_ContactInformationChanged;
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
            bool found = false;

            try
            {               
                lyncClient = LyncClient.GetClient();                

                foreach (Group group in lyncClient.ContactManager.Groups)
                {
                    foreach (Contact contact in group)
                    {
                        // If the selected User Id is found  we set the event and get the current location
                        if (contact.Uri.Contains(userId))
                        {
                            selectedContact = contact;
                            selectedContact.ContactInformationChanged += contactInformationChanged;

                            currentLocation = selectedContact.GetContactInformation(ContactInformationType.LocationName).ToString();

                            found = true;

                            break;
                        }
                    }
                }

                // If the User Id is not found we need to disable the event
                if (found == false)
                {
                    ErrorLog.AddError(ErrorType.Failure, Strings.Lync_NoUserId);
                    e.Cancel = true;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.Lync_ApplicationNotFound);
                Logger.Write(ex);
                e.Cancel = true;
            }
        }

        protected override void OnDisabled(DisabledEventArgs e)
        {
            lyncClient = null;

            if (selectedContact != null)
            {
                selectedContact.ContactInformationChanged -= contactInformationChanged;
                selectedContact = null;
            }
        }

        private void Contact_ContactInformationChanged(object sender, ContactInformationChangedEventArgs e)
        {
            var contact = sender as Contact;
            string selectedLocation = contact.GetContactInformation(ContactInformationType.LocationName).ToString();

            // If the location changed event is triggered and the new location name is different from the previous one, we trigger the event
            if (e.ChangedContactInformation.Contains(ContactInformationType.LocationName) && !currentLocation.ToLower().Equals(selectedLocation.ToLower()))
            {
                Trigger();

                currentLocation = selectedLocation;
            }
        }

        #region IWpfConfigurable Methods

        public WpfConfiguration ConfigurationControl
        {
            get { return new LyncSelectUserConfig(userId); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            LyncSelectUserConfig config = configurationControl as LyncSelectUserConfig;

            userId = config.UserId;
        }

        #endregion

        #region IConfigurable Members

        public string GetConfigString()
        {
            return string.Format(CultureInfo.CurrentCulture, Strings.Lync_SelectUserConfigString, userId);
        }

        #endregion
    }
}
