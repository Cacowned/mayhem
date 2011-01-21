
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
        public ActionBase Action { get; set; }
        public ReactionBase Reaction { get; set; }
    }
}
