using System;
using System.Globalization;
using System.Runtime.Serialization;
using MayhemCore;
using MayhemWpf.ModuleTypes;
using MayhemWpf.UserControls;
using Microsoft.Lync.Model;
using OfficeModules.Resources;
using OfficeModules.Wpf;

namespace OfficeModules.Events.Lync
{
    /// <summary>
    /// An event that will be triggered when the location of a predefined contact changes.
    /// </summary>
    [DataContract]
    [MayhemModule("Lync: Location Changed", "Triggers when the location changes")]
    public class LyncLocationChanged : EventBase, IWpfConfigurable
    {
        /// <summary>
        /// The User ID of the predefined contact.
        /// </summary>
        [DataMember]
        private string userId;

        private LyncClient lyncClient;
        private Self self;
        private Contact selectedContact;
        private string currentLocation;

        private EventHandler<ContactInformationChangedEventArgs> contactInformationChanged;

        /// <summary>
        /// This method is called after the event is loaded.
        /// </summary>
        protected override void OnAfterLoad()
        {
            lyncClient = null;
            selectedContact = null;

            contactInformationChanged = Contact_ContactInformationChanged;
        }

        /// <summary>
        /// This method gets the Lync Client instance and is subscribing to the ContactInformationChangedEvent.
        /// </summary>
        protected override void OnEnabling(EnablingEventArgs e)
        {
            try
            {
                lyncClient = LyncClient.GetClient();

                self = lyncClient.Self;

                if (self == null)
                {
                    ErrorLog.AddError(ErrorType.Failure, Strings.Lync_NotLoggedIn);
                    return;
                }

                try
                {
                    selectedContact = self.Contact.ContactManager.GetContactByUri(userId);
                    selectedContact.ContactInformationChanged += contactInformationChanged;

                    currentLocation = selectedContact.GetContactInformation(ContactInformationType.LocationName).ToString();
                }
                catch (Exception ex)
                {
                    ErrorLog.AddError(ErrorType.Failure, Strings.Lync_NoUserId);
                    Logger.Write(ex);
                    e.Cancel = true;

                    return;
                }
            }
            catch (Exception ex)
            {
                ErrorLog.AddError(ErrorType.Failure, Strings.Lync_ApplicationNotFound);
                Logger.Write(ex);
                e.Cancel = true;
            }
        }

        /// <summary>
        /// This method is unsubscribing from the ContactInformationChangedEvent.
        /// </summary>
        protected override void OnDisabled(DisabledEventArgs e)
        {
            lyncClient = null;

            if (selectedContact != null)
            {
                selectedContact.ContactInformationChanged -= contactInformationChanged;
                selectedContact = null;
            }
        }

        /// <summary>
        /// This method is called when the ContactInformationChangedEvent is triggered, and if the type of the event is ContactInformationType.LocationName will trigger this event.
        /// </summary>
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
            get { return new LyncSelectUserConfig(userId, Strings.LyncLocationChanged_Title); }
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
