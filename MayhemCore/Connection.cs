
using System;
namespace MayhemCore
{
    /// <summary>
    /// This class will be a pairing of an action and reaction
    /// </summary>
    public class Connection
    {
        protected bool enabled;
        public bool Enabled {
            get {
                return enabled;
            }
            set {
                if (value == true && enabled == false)
                {
                    Action.Enable();
                    Reaction.Enable();
                }
                else if (value == false && enabled == true)
                {
                    Action.Disable();
                    Reaction.Disable();
                }
            }
        }
        public ActionBase Action { get; private set; }
        public ReactionBase Reaction { get; private set; }

        public Connection(ActionBase action, ReactionBase reaction) {
            this.Action = action;
            this.Reaction = reaction;

            this.Action.OnActionActivated += this.action_activated;
        }

        public void action_activated(object sender, EventArgs e) {
            // If we got into this method call, we probably don't need
            // to check if we are enabled.
            if (Enabled)
                Reaction.Perform();
        }
    }
}
