﻿using System;
using System.Runtime.Serialization;
using System.Threading;

namespace MayhemCore
{
    /// <summary>
    /// This class will be a pairing of an event and reaction
    /// </summary>
    [DataContract]
    public class Connection
    {
        /// <summary>
        /// True if this connection is enabled
        /// false if disabled.
        /// </summary>
        [DataMember]
        public bool Enabled { get; private set; }

        /// <summary>
        /// The event that this connection is using
        /// </summary>
        [DataMember]
        public EventBase Event { get; private set; }

        /// <summary>
        /// The reaction that this connection is using
        /// </summary>
        [DataMember]
        public ReactionBase Reaction { get; private set; }

        private bool isConfiguring;
        public bool IsConfiguring 
        {
            get
            {
                return isConfiguring;
            }
            set
            {
                isConfiguring = value;
                Event.IsConfiguring = value;
                Reaction.IsConfiguring = value;
            }
        }

        public Connection() { }

        /// <summary>
        /// Create a new connection
        /// </summary>
        /// <param name="e">The event to trigger on</param>
        /// <param name="reaction">The reaction to perform</param>
        public Connection(EventBase e, ReactionBase reaction)
        {
            // Set our event and reactions 
            this.Event = e;
            this.Reaction = reaction;

            // Set them to have a reference to this connection
            this.Event.connection = this;
            this.Reaction.connection = this;

            // Set up the event handler for when the event triggers
            this.Event.EventActivated += this.OnEventActivated;
        }

        /// <summary>
        /// Calls the Reaction when the event gets triggered.
        /// </summary>
        private void OnEventActivated(object sender, EventArgs e)
        {
            // If we got into this method call, we probably don't need
            // to check if we are enabled.
            if (Enabled)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback((o) => Reaction.Perform()));
            }
        }

        /// <summary>
        /// Enable this connection's
        /// event and reaction
        /// </summary>
        public void Enable(Action actionOnComplete)
        {
            // if we are already enabled, just stop
            if (Enabled)
            {
                if (actionOnComplete != null)
                {
                    actionOnComplete();
                }
                return;
            }
            _Enable(actionOnComplete);
        }

        private void _Enable(Action actionOnComplete)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback((o) => EnableOnThread(actionOnComplete)));
        }

        private void EnableOnThread(Action actionOnComplete)
        {
            if (!Event.Enabled)
            {
                // Enable the event
                Event.Enable();
                // If the event didn't enable
                if (!Event.Enabled)
                {
                    Enabled = false;
                    // Return out and don't try to 
                    // enable the reaction
                    return;
                }
            }
            if (!Reaction.Enabled)
            {
                Reaction.Enable();
                if (!Reaction.Enabled)
                {
                    /* If we got here, then it means
                     * the event is enabled
                     * and the reaction won't enable
                     * 
                     * we need to disable the event
                     * and then return out
                     */
                    Event.Disable();

                    Enabled = false;
                    // Now return out
                    return;
                }
            }
            /* Double check that both are enabled
             * We shouldn't be able to get here if they aren't
             */
            if (Event.Enabled && Reaction.Enabled)
            {
                Enabled = true;
                if (actionOnComplete != null)
                {
                    actionOnComplete();
                }
            }
        }

        public void Disable(Action actionOnComplete)
        {
            // if we aren't already enabled, just stop
            if (!Enabled)
            {
                if (actionOnComplete != null)
                {
                    actionOnComplete();
                }
                return;
            }

            ThreadPool.QueueUserWorkItem(new WaitCallback((o) => DisableOnThread(actionOnComplete)));            
        }

        private void DisableOnThread(Action actionOnComplete)
        {
            if (Event.Enabled)
            {
                Event.Disable();
            }
            if (Reaction.Enabled)
            {
                Reaction.Disable();
            }

            /* This might need to be changed to be more like
             * Enable() if we decide that modules can
             * decide not to shut down. That doesn't
             * seem like a reasonable option though, so for
             * now, this will stay as it is
             */
            Enabled = false;
            if (actionOnComplete != null)
            {
                actionOnComplete();
            }
        }

        public void Delete()
        {
            if (Enabled)
            {
                Disable(null);
            }

            Event.Delete();
            Reaction.Delete();
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            Event.connection = this;
            Reaction.connection = this;

            // Set up the event handler for when the event triggers
            this.Event.EventActivated += this.OnEventActivated;

            // If we have started up and are enabled, then we need to
            // actually enable our events and reactions
            if (Enabled)
                _Enable(null);
        }
    }
}
