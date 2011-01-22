
using System;
namespace MayhemCore
{
    /// <summary>
    /// Base class for all action modules
    /// </summary>
    public abstract class ActionBase: ModuleBase
    {
        public delegate void ActionActivateHandler(object sender, EventArgs e);
        public virtual event ActionActivateHandler OnActionActivated;

        public ActionBase(string name, string description)
            :base(name, description)
        {

        }
    }
}
