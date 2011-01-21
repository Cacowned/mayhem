
namespace MayhemCore
{
    /// <summary>
    /// This class is extended by ActionBase and ReactionBase
    /// </summary>
    public abstract class ModuleBase
    {
        protected bool hasConfig = false;

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
        // category

        public void ShowConfig() { }

        public void Enable() { }
        public void Disable() { }

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
