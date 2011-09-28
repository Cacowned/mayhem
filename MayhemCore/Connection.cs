using System;
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
        public bool IsEnabled
        {
            get;
            private set; 
        }

        /// <summary>
        /// The event that this connection is using
        /// </summary>
        [DataMember]
        public EventBase Event
        {
            get;
            private set; 
        }

        /// <summary>
        /// The reaction that this connection is using
        /// </summary>
        [DataMember]
        public ReactionBase Reaction
        {
            get;
            private set;
        }

        private bool isConfiguring;
        public bool IsConfiguring
        {
            get
            {
                return isConfiguring;
            }
            internal set
            {
                isConfiguring = value;
                Event.IsConfiguring = value;
                Reaction.IsConfiguring = value;
            }
        }

        /// <summary>
        /// Create a new connection
        /// </summary>
        /// <param name="e">The event to trigger on</param>
        /// <param name="reaction">The reaction to perform</param>
        internal Connection(EventBase e, ReactionBase reaction)
        {
            // Set our event and reactions 
            this.Event = e;
            this.Reaction = reaction;

            // Set them to have a reference to this connection
            this.Event.Connection = this;
            this.Reaction.Connection = this;

            // Set up the event handler for when the event triggers
            this.Event.EventActivated += this.Trigger;
        }

        /// <summary>
        /// Calls the Reaction when the event gets triggered.
        /// </summary>
        private void Trigger(object sender, EventArgs e)
        {
            // If we got into this method call, we probably don't need
            // to check if we are enabled.
            if (IsEnabled)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback((o) => Reaction.Perform()));
            }
        }

        /// <summary>
        /// Enable this connection's event and reaction
        /// </summary>
        public void Enable(Action actionOnComplete)
        {
            // if we are already enabled, just stop
            if (IsEnabled)
            {
                if (actionOnComplete != null)
                {
                    actionOnComplete();
                }
                return;
            }
            Enable_(actionOnComplete);
        }

        private void Enable_(Action actionOnComplete)
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback((o) => EnableOnThread(actionOnComplete)));
        }

        private void EnableOnThread(Action actionOnComplete)
        {
            if (!Event.IsEnabled)
            {
                // Enable the event
                Event.Enable();
            }
            // If the event is not enabled we don't try to enable the reaction
            if (Event.IsEnabled && !Reaction.IsEnabled)
            {
                try
                {
                    Reaction.Enable();
                }
                catch
                {
                    ErrorLog.AddError(ErrorType.Failure, "Error enabling " + Reaction.Name);
                }
                if (!Reaction.IsEnabled)
                {
                    // If we got here, then it means the event is enabled and the reaction won't enable.
                    // We need to disable the event and then return out
                    Event.Disable();
                }
            }
            // Double check that both are enabled. We shouldn't be able to get here if they aren't.
            IsEnabled = Event.IsEnabled && Reaction.IsEnabled;
            if (actionOnComplete != null)
            {
                actionOnComplete();
            }
        }

        public void Disable(Action actionOnComplete)
        {
            // if we aren't already enabled, just stop
            if (!IsEnabled)
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
            if (Event.IsEnabled)
            {
                try
                {
                    Event.Disable();
                }
                catch
                {
                    ErrorLog.AddError(ErrorType.Failure, "Error disabling " + Event.Name);
                }
            }
            if (Reaction.IsEnabled)
            {
                try
                {
                    Reaction.Disable();
                }
                catch
                {
                    ErrorLog.AddError(ErrorType.Failure, "Error enabling " + Reaction.Name);
                }
            }

            /* This might need to be changed to be more like
             * Enable() if we decide that modules can
             * decide not to shut down. That doesn't
             * seem like a reasonable option though, so for
             * now, this will stay as it is
             */
            IsEnabled = false;
            if (actionOnComplete != null)
            {
                actionOnComplete();
            }
        }

        internal void Delete()
        {
            if (IsEnabled)
            {
                Disable(null);
            }

            Event.Delete();
            Reaction.Delete();
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            Event.Connection = this;
            Reaction.Connection = this;

            // Set up the event handler for when the event triggers
            this.Event.EventActivated += this.Trigger;

            // If we have started up and are enabled, then we need to
            // actually enable our events and reactions
            if (IsEnabled)
                Enable_(null);
        }
    }
}
