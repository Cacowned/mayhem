
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
                // TODO: What if they are already enabled? We need to check that.
                if (value == true)
                {
                    Action.Enable();
                    Reaction.Enable();
                }
                else
                {
                    Action.Disable();
                    Reaction.Disable();
                }
            }
        }
        public ActionBase Action { get; set; }
        public ReactionBase Reaction { get; set; }
    }
}
