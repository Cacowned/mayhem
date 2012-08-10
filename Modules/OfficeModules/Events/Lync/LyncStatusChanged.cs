using System;
using System.Collections.Generic;
using MayhemCore;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Group;
using OfficeModules.Resources;

namespace OfficeModules.Events.Lync
{
    /// <summary>
    /// An event that will be triggered when the status of any contact changes.
    /// </summary>
    [MayhemModule("Lync: Status Changed", "Triggers when the status of a contact changes")]
    public class LyncStatusChanged : EventBase
    {
        private LyncClient lyncClient;
        private EventHandler<ContactInformationChangedEventArgs> contactInformationChanged;

        protected override void OnAfterLoad()
        {
            lyncClient = null;

            contactInformationChanged = Contact_ContactInformationChanged;
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
            try
            {
                lyncClient = LyncClient.GetClient();
                Dictionary<String, ContactSubscription> contactSubscriptions = new Dictionary<String, ContactSubscription>();

                foreach (Group group in lyncClient.ContactManager.Groups)
                {
                    foreach (Contact contact in group)
                    {
                        contact.ContactInformationChanged += contactInformationChanged;
                    }
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
            if (lyncClient != null)
            {
                foreach (Group group in lyncClient.ContactManager.Groups)
                {
                    foreach (Contact contact in group)
                    {
                        contact.ContactInformationChanged -= contactInformationChanged;
                    }
                }

                lyncClient = null;
            }
        }

        /// <summary>
        /// This method is called when the ContactInformationChangedEvent is triggered, and will trigger this event if the status has changed.
        /// </summary>
        private void Contact_ContactInformationChanged(object sender, ContactInformationChangedEventArgs e)
        {
            var contact = sender as Contact;

            if (e.ChangedContactInformation.Contains(ContactInformationType.Activity))
            {
                Trigger();
            }
        }
    }
}
