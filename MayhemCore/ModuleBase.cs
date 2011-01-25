
namespace MayhemCore
{
    /// <summary>
    /// This class is extended by ActionBase and ReactionBase
    /// </summary>
    public abstract class ModuleBase
    {
        protected bool hasConfig = false;
        /// <summary>
        /// Whether this module has configuration settings
        /// </summary>
        public bool HasConfig {
            get {
                return hasConfig;
            }
        }


        public string Name
        {
            get;
            protected set;
        }
        
        public string Description
        {
            get;
            protected set;
        }
        
        // TODO: category?


        public virtual void Enable() { }
        public virtual void Disable() { }

        public ModuleBase(string name, string description)
        {
            this.Name = name;
            this.Description = description;
        }

        public override string ToString() {
            return Name;
        }
    }
}
