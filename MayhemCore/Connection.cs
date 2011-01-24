
using System;
using System.Threading;
namespace MayhemCore
{
    /// <summary>
    /// This class will be a pairing of an action and reaction
    /// </summary>
    public class Connection
    {
        protected Thread actionThread;
        protected Thread reactionThread;

        protected bool enabled;
        public bool Enabled {
            get;
            private set;
        }
        public ActionBase Action { get; private set; }
        public ReactionBase Reaction { get; private set; }

        public Connection(ActionBase action, ReactionBase reaction) {
            this.Action = action;
            this.Reaction = reaction;

            this.Action.ActionActivated += this.action_activated;
        }

        public void action_activated(object sender, EventArgs e) {
            // If we got into this method call, we probably don't need
            // to check if we are enabled.
            if (Enabled)
                Reaction.Perform();
        }


        public void Enable() {
            if (this.Enabled) {
                return;
            }

            Action.Enable();
            Reaction.Enable();

            this.Enabled = true;
        }

        public void Disable() {
            if (!this.Enabled) { 
                return;
            }

            Action.Disable();
            Reaction.Disable();

            this.Enabled = false;
        }
    }
}
