
using System;
using System.Runtime.Serialization;
using System.Diagnostics;
namespace MayhemCore
{
	/// <summary>
	/// This class will be a pairing of an action and reaction
	/// </summary>
	[Serializable]
	public class Connection : ISerializable
	{
		public bool Enabled {
			get;
			private set;
		}
		public ActionBase Action { get; set; }
		public ReactionBase Reaction { get; set; }

		public Connection() { }

		public Connection(ActionBase action, ReactionBase reaction) {
			this.Action = action;
			this.Reaction = reaction;

			this.Action.connection = this;
			this.Reaction.connection = this;

			// Set up the event handler for when the action triggers
			this.Action.ActionActivated += this.action_activated;
		}

        // TODO: I haven't figured out if this 
        // is actually working properly.
        // Needs more testing
		~Connection() {
            try
            {
                Action.Disable();
                Reaction.Disable();
                Action = null;
                Reaction = null;
            }
            catch (NullReferenceException e)
            {
                Debug.WriteLine("Null Reference Exception " + e);
            }
		}

		/// <summary>
		/// Calls the Reaction when the action gets triggered.
		/// </summary>
		public void action_activated(object sender, EventArgs e) {
			// If we got into this method call, we probably don't need
			// to check if we are enabled.
			if (Enabled)
				Reaction.Perform();
		}

		/// <summary>
		/// Enable this connection's
		/// action and reaction
		/// </summary>
		public void Enable() {
			// if we are already enabled, just stop
			if (this.Enabled) {
				return;
			}

            // Enable the action
			Action.Enable();
            // If the action didn't enable
            if(!Action.Enabled) {
                // Return out and don't try to 
                // enable the reaction
                return;
            }

			Reaction.Enable();
            if(!Reaction.Enabled) {
                /* If we got here, then it means
                 * the action is enabled
                 * and the reacion won't enable
                 * 
                 * we need to disable the action
                 * and then return out
                 */
                Action.Disable();

                // Now return out
                return;
            }

            /* Double check that both are enabled
             * We shouldn't be able to get here if they aren't
             */
            if (Action.Enabled && Reaction.Enabled)
            {
                this.Enabled = true;
            }
		}

		public void Disable() {
			// if we aren't already enabled, just stop
			if (!this.Enabled) {
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
			this.Enabled = false;
		}

		#region Serialization
		public Connection(SerializationInfo info, StreamingContext context) {
			Action = info.GetValue("Action", typeof(object)) as ActionBase;
			Reaction = info.GetValue("Reaction", typeof(object)) as ReactionBase;

			Action.connection = this;
			Reaction.connection = this;

			bool enabled = info.GetBoolean("Enabled");
			if (enabled)
				this.Enable();
		}

		public virtual void GetObjectData(SerializationInfo info, StreamingContext context) {
			info.AddValue("Action", Action);
			info.AddValue("Reaction", Reaction);
			info.AddValue("Enabled", Enabled);
		}

		#endregion
	}
}
