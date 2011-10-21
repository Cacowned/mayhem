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
        /// True if this connection is enabled. False if disabled.
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
        internal EventBase Event
        {
            get;
            private set;
        }

        /// <summary>
        /// The reaction that this connection is using
        /// </summary>
        [DataMember]
        internal ReactionBase Reaction
        {
            get;
            private set;
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
        }

        /// <summary>
        /// Calls the Reaction when the event gets triggered.
        /// </summary>
        internal void Trigger()
        {
            // If we got into this method call, we probably don't need
            // to check if we are enabled.
            if (IsEnabled)
            {
                ThreadPool.QueueUserWorkItem(o => Reaction.Perform());
            }
        }

        /// <summary>
        /// Enable this connection's event and reaction
        /// </summary>
        internal void Enable(EnablingEventArgs e, Action actionOnComplete)
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

            Enable_(e, actionOnComplete);
        }

        private void Enable_(EnablingEventArgs e, Action actionOnComplete)
        {
            ThreadPool.QueueUserWorkItem(o => EnableOnThread(e, actionOnComplete));
        }

        private void EnableOnThread(EnablingEventArgs e, Action actionOnComplete)
        {
            if (!Event.IsEnabled)
            {
                // Enable the event
                Event.Enable(e);
            }

            // If the event is not enabled we don't try to enable the reaction
            if (Event.IsEnabled && !Reaction.IsEnabled)
            {
                try
                {
                    Reaction.Enable(e);
                }
                catch
                {
                    ErrorLog.AddError(ErrorType.Failure, "Error enabling " + Reaction.Name);
                }

                if (!Reaction.IsEnabled)
                {
                    // If we got here, then it means the event is enabled and the reaction won't enable.
                    // We need to disable the event and then return out
                    Event.Disable(new DisabledEventArgs(e.WasConfiguring));
                }
            }

            // Double check that both are enabled. We shouldn't be able to get here if they aren't.
            IsEnabled = Event.IsEnabled && Reaction.IsEnabled;
            if (actionOnComplete != null)
            {
                actionOnComplete();
            }
        }

        internal void Disable(DisabledEventArgs e, Action actionOnComplete)
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

            ThreadPool.QueueUserWorkItem(o => DisableOnThread(e, actionOnComplete));
        }

        private void DisableOnThread(DisabledEventArgs e, Action actionOnComplete)
        {
            if (Event.IsEnabled)
            {
                try
                {
                    Event.Disable(e);
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
                    Reaction.Disable(e);
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
                Disable(new DisabledEventArgs(false), null);
            }

            Event.Delete();
            Reaction.Delete();
        }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            Event.Connection = this;
            Reaction.Connection = this;

            // If we have started up and are enabled, then we need to
            // actually enable our events and reactions
            if (IsEnabled)
                Enable_(new EnablingEventArgs(false), null);
        }
    }
}
