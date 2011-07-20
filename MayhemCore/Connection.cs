
using System;
using System.Runtime.Serialization;
namespace MayhemCore
{
	/// <summary>
	/// This class will be a pairing of an action and reaction
	/// </summary>
	[DataContract]
	public class Connection
	{
		/// <summary>
		/// True if this connection is enabled
		/// false if disabled.
		/// </summary>
        [DataMember]
		public bool Enabled { get;	private set; }

		/// <summary>
		/// The action that this connection is using
		/// </summary>
        [DataMember]
		public ActionBase Action { get;  private set; }

		/// <summary>
		/// The reaction that this connection is using
		/// </summary>
        [DataMember]
		public ReactionBase Reaction { get; private set; }

		public Connection() { }

		/// <summary>
		/// Create a new connection
		/// </summary>
		/// <param name="action">The action to trigger on</param>
		/// <param name="reaction">The reaction to perform</param>
		public Connection(ActionBase action, ReactionBase reaction) 
        {
			// Set our action and reactions 
			this.Action = action;
			this.Reaction = reaction;

			// Set them to have a reference to this connection
			this.Action.connection = this;
			this.Reaction.connection = this;

			// Set up the event handler for when the action triggers
			this.Action.ActionActivated += this.OnActionActivated;
		}

		/// <summary>
		/// Calls the Reaction when the action gets triggered.
		/// </summary>
		private void OnActionActivated(object sender, EventArgs e)
        {
			// If we got into this method call, we probably don't need
			// to check if we are enabled.
			if (Enabled)
				Reaction.Perform();
		}

		/// <summary>
		/// Enable this connection's
		/// action and reaction
		/// </summary>
		public void Enable() 
        {
			// if we are already enabled, just stop
			if (Enabled) 
            {
				return;
			}
            _Enable();
		}

        private void _Enable()
        {
            // Enable the action
            Action.Enable();
            // If the action didn't enable
            if (!Action.Enabled)
            {
                Enabled = false;
                // Return out and don't try to 
                // enable the reaction
                return;
            }

            Reaction.Enable();
            if (!Reaction.Enabled)
            {
                /* If we got here, then it means
                 * the action is enabled
                 * and the reaction won't enable
                 * 
                 * we need to disable the action
                 * and then return out
                 */
                Action.Disable();

                Enabled = false;
                // Now return out
                return;
            }

            /* Double check that both are enabled
             * We shouldn't be able to get here if they aren't
             */
            if (Action.Enabled && Reaction.Enabled)
            {
                Enabled = true;
            }
        }

		public void Disable() 
        {
			// if we aren't already enabled, just stop
			if (!Enabled)
            {
				return;
			}

			Action.Disable();
			Reaction.Disable();

            /* This might need to be changed to be more like
             * Enable() if we decide that modules can
             * decide not to shut down. That doesn't
             * seem like a reasonable option though, so for
             * now, this will stay as it is
             */
			Enabled = false;
		}

        [OnDeserialized]
        public void OnDeserialized(StreamingContext context)
        {
            Action.connection = this;
            Reaction.connection = this;

            // Set up the event handler for when the action triggers
            this.Action.ActionActivated += this.OnActionActivated;

            // If we have started up and are enabled, then we need to
            // actually enable our actions and reactions
            if(Enabled)
                _Enable();
        }

	}
}
