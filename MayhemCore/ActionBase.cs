
using System;
namespace MayhemCore
{
    public delegate void ActionActivateHandler(object sender, EventArgs e);

    /// <summary>
    /// Base class for all action modules
    /// </summary>
    public abstract class ActionBase: ModuleBase
    {
        /// <summary>
        /// Event that triggers when the action is activated
        /// </summary>
        public event ActionActivateHandler ActionActivated;

        public ActionBase(string name, string description)
            :base(name, description)
        {
        }

        /// <summary>
        /// Event trigger for when the action is activated. This shouldn't
        /// need to be overridden, just attached to
        /// </summary>
        protected virtual void OnActionActivated() {
            ActionActivateHandler handler = ActionActivated;
            if (handler != null) {
                handler(this, null);
            }
        }
    }
}
