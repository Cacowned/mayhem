using System;
using MayhemCore;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Conversation;
using OfficeModules.Resources;

namespace OfficeModules.Events.Lync
{
    /// <summary>
    /// An event that will be triggered when an instant message is received.
    /// </summary>
    [MayhemModule("Lync: Instant Message Received", "Triggers when an instant message is received only if the IM window of the sender is not already opened")]
    public class LyncInstantMessageReceived : EventBase
    {
        private LyncClient lyncClient;
        private ConversationManager conversationManager;

        private EventHandler<ConversationManagerEventArgs> conversationAdded;

        protected override void OnAfterLoad()
        {
            lyncClient = null;
            conversationManager = null;

            conversationAdded = Conversations_ConversationAdded;
        }

        protected override void OnEnabling(EnablingEventArgs e)
        {
            try
            {
                lyncClient = LyncClient.GetClient();
                conversationManager = lyncClient.ConversationManager;

                conversationManager.ConversationAdded += conversationAdded;
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
            if (conversationManager != null)
            {
                conversationManager.ConversationAdded -= conversationAdded;
                conversationManager = null;
            }

            if (lyncClient != null)
            {
                lyncClient = null;
            }
        }

        /// <summary>
        /// This method is called when the ConversationManagerEvent is triggered, and will trigger this event if an instant message is received.
        /// </summary>
        void Conversations_ConversationAdded(object sender, ConversationManagerEventArgs e)
        {
            if (e.Conversation.Modalities[ModalityTypes.InstantMessage].State == ModalityState.Notified)
            {
                Trigger();
            }
        }
    }
}
