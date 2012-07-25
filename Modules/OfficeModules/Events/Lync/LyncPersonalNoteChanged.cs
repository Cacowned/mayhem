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
    /// An event that will be triggered when the personal note of a predefined contact changes.
    /// </summary>
    [DataContract]
    [MayhemModule("Lync: Personal Note Changed", "Triggers when the personal note changes")]
    public class LyncPersonalNoteChanged : EventBase, IWpfConfigurable
    {
        /// <summary>
        /// The User ID of the predefined contact that will be monitored.
        /// </summary>
        [DataMember]
        private string userId;

        private LyncClient lyncClient;
        private Self self;
        private string currentPersonalNote;
        private Contact selectedContact;

        private EventHandler<ContactInformationChangedEventArgs> contactInformationChanged;

        protected override void OnAfterLoad()
        {
            lyncClient = null;
            selectedContact = null;

            contactInformationChanged = Contact_ContactInformationChanged;
        }

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

                    object personalNoteObject = selectedContact.GetContactInformation(ContactInformationType.PersonalNote);

                    if(personalNoteObject == null)
                    {
                        currentPersonalNote = "";
                    }
                    else
                    {
                        currentPersonalNote = personalNoteObject.ToString();
                    }
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
        /// This method is called when the ContactInformationChangedEvent is triggered, and will trigger this event if the personal note of the monitored user changes.
        /// </summary>
        private void Contact_ContactInformationChanged(object sender, ContactInformationChangedEventArgs e)
        {
            var contact = sender as Contact;
            string selectedPersonalNote;
            object personalNoteObject = contact.GetContactInformation(ContactInformationType.PersonalNote);

            if (personalNoteObject == null)
            {
                selectedPersonalNote = "";
            }
            else
            {
                selectedPersonalNote = personalNoteObject.ToString();
            }

            // If the new personal note is different from the previous one, we trigger the event.
            if (e.ChangedContactInformation.Contains(ContactInformationType.PersonalNote) && !currentPersonalNote.ToString().ToLower().Equals(selectedPersonalNote.ToLower()))
            {
                Trigger();

                currentPersonalNote = selectedPersonalNote;
            }
        }

        #region IWpfConfigurable Methods

        public WpfConfiguration ConfigurationControl
        {
            get { return new LyncSelectUserConfig(userId, Strings.LyncPersonalNoteChanged_Title); }
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
