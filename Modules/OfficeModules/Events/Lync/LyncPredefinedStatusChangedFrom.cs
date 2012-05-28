﻿using System;
using System.Collections.Generic;
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
    /// This event is triggered when the status of a predefined contact changes from the one that is setted by the user to any other
    /// </summary>
    [DataContract]
    [MayhemModule("Lync: Status Changed From", "Triggers when the status of a predefined contact changes from the predefined status")]
    public class LyncPredefinedStatusChangedFrom : EventBase, IWpfConfigurable
    {
        [DataMember]
        private string status;

        [DataMember]
        private string userId;

        private LyncClient lyncClient;
        private string currentStatus;
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
                Dictionary<String, ContactSubscription> contactSubscriptions = new Dictionary<String, ContactSubscription>();
                bool found = false;

                foreach (Group group in lyncClient.ContactManager.Groups)
                {
                    foreach (Contact contact in group)
                    {
                        if (contact.Uri.Contains(userId))
                        {
                            selectedContact = contact;
                            selectedContact.ContactInformationChanged += contactInformationChanged;
                           
                            currentStatus = selectedContact.GetContactInformation(ContactInformationType.Activity).ToString();
                            
                            found = true;

                            break;
                        }
                    }
                }

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

            if (e.ChangedContactInformation.Contains(ContactInformationType.Activity))
            {
                string contactStatus = contact.GetContactInformation(ContactInformationType.Activity).ToString();

                if ((status.ToLower().Equals("any") || currentStatus.ToLower().Equals(status.ToLower())) && !contactStatus.ToLower().Equals(status.ToLower()))
                {
                    Logger.WriteLine(contactStatus);
                    Trigger();
                }

                currentStatus = contactStatus;
            }
        }

        #region IWpfConfigurable Methods

        public WpfConfiguration ConfigurationControl
        {
            get { return new LyncStatusChangedConfig(userId, status); }
        }

        public void OnSaved(WpfConfiguration configurationControl)
        {
            LyncStatusChangedConfig config = configurationControl as LyncStatusChangedConfig;

            status = config.Status;
            userId = config.UserId;
        }

        #endregion

        #region IConfigurable Members

        public string GetConfigString()
        {
            return string.Format(CultureInfo.CurrentCulture, Strings.Lync_StatusChangedConfigString, userId, status);
        }

        #endregion
    }
}
