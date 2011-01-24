
using System;
namespace MayhemCore
{
    public delegate void ActionActivateHandler(object sender, EventArgs e);

    /// <summary>
    /// Base class for all action modules
    /// </summary>
    public abstract class ActionBase: ModuleBase
    {
        public event ActionActivateHandler ActionActivated;

        public ActionBase(string name, string description)
            :base(name, description)
        {
        }

        protected virtual void OnActionActivated() {
            ActionActivateHandler handler = ActionActivated;
            if (handler != null) {
                handler(this, null);
            }
        }
    }
}
