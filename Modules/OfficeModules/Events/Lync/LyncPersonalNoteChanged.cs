﻿using System;
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
    /// This event is triggered when the personal note of a predefined contact changes
    /// </summary>
    [DataContract]
    [MayhemModule("Lync: Personal Note Changed", "Triggers when the personal note changes")]
    public class LyncPersonalNoteChanged : EventBase, IWpfConfigurable
    {
        [DataMember]
        private string userId;

        private LyncClient lyncClient;
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
            bool found = false;

            try
            {
                lyncClient = LyncClient.GetClient();

                foreach (Group group in lyncClient.ContactManager.Groups)
                {
                    foreach (Contact contact in group)
                    {
                        // If the selected User Id is found  we set the event and get the current personal note
                        if (contact.Uri.Contains(userId))
                        {                           
                            selectedContact = contact;
                            selectedContact.ContactInformationChanged += contactInformationChanged;

                            currentPersonalNote = selectedContact.GetContactInformation(ContactInformationType.PersonalNote).ToString();
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
            string selectedPersonalNote = contact.GetContactInformation(ContactInformationType.PersonalNote).ToString();

            // If the location changed event is triggered and the new personal note is different from the previous one, we trigger the event
            if (e.ChangedContactInformation.Contains(ContactInformationType.PersonalNote) && !currentPersonalNote.ToString().ToLower().Equals(selectedPersonalNote.ToLower()))
            {
                Trigger();

                currentPersonalNote = selectedPersonalNote;
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