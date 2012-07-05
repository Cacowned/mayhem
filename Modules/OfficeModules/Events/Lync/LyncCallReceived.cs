﻿using System;
using MayhemCore;
using Microsoft.Lync.Model;
using Microsoft.Lync.Model.Conversation;
using OfficeModules.Resources;

namespace OfficeModules.Events.Lync
{
    /// <summary>
    /// An event that will be triggered when a call is received.
    /// </summary>
    [MayhemModule("Lync: Call Received", "Triggers when a call is received")]
    public class LyncCallReceived : EventBase
    {
        private LyncClient lyncClient;
        private ConversationManager conversationManager;

        private EventHandler<ConversationManagerEventArgs> conversationAdded;

        /// <summary>
        /// This method is called after the event is loaded.
        /// </summary>
        protected override void OnAfterLoad()
        {
            lyncClient = null;
            conversationManager = null;

            conversationAdded = Conversations_ConversationAdded;
        }

        /// <summary>
        /// This method gets the Lync Client instance and is subscribing to the ConversationManagerEvent.
        /// </summary>
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

        /// <summary>
        /// This method is unsubscribing from the ConversationManagerEvent.
        /// </summary>
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
        /// This method is called when the ConversationManagerEvent is triggered, and if the type of the event is ModalityTypes.AudioVideo will trigger this event.
        /// </summary>
        void Conversations_ConversationAdded(object sender, ConversationManagerEventArgs e)
        {
            if (e.Conversation.Modalities[ModalityTypes.AudioVideo].State == ModalityState.Notified)
            {
                Trigger();
            }
        }
    }
}
