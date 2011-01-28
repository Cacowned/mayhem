
using System;
using System.Threading;
using System.Runtime.Serialization;
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
            set;
        }
        public ActionBase Action { get; set; }
        public ReactionBase Reaction { get; set; }

        public Connection() { }

        public Connection(ActionBase action, ReactionBase reaction) {
            this.Action = action;
            this.Reaction = reaction;

            // Set up the event handler for when the action triggers
            this.Action.ActionActivated += this.action_activated;
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

            Action.Enable();
            Reaction.Enable();

            this.Enabled = true;
        }

        public void Disable() {
            // if we aren't already enabled, just stop
            if (!this.Enabled) { 
                return;
            }

            Action.Disable();
            Reaction.Disable();

            this.Enabled = false;
        }

        #region Serialization
        public Connection(SerializationInfo info, StreamingContext context)
        {
            Action = info.GetValue("Action", typeof(object)) as ActionBase;
            Reaction = info.GetValue("Reaction", typeof(object)) as ReactionBase;

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
